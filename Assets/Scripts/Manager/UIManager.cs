using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum UIType : byte
{ 
    None,
    TitleUI,
    CharacterSelectUI,
    InGameUI,
    HuntingAreaSelectUI,
    ShopUI,
    InformationPopup,
    CharacterCreatePopup,
    ConfirmPopup,
    GameMenuPopup,
    InventoryPopup,
    SkillTreePopup,
    EquipmentPopup
}

public enum UIRootType : byte
{
    None,
    BackGround,
    Main,
    Content,
    Popup,
    Front,
    ETC
}

// UI로드, 화면에 붙이기, UI 켜고 끄기 기능 ~ 켜고 꺼지는 타이밍은 외부에서 결정될 예정
public class UIManager : BaseMonoManager<UIManager>
{
    [SerializeField] private Canvas Canvas_BackGround;
    [SerializeField] private Canvas Canvas_Main;  // 완전히 전환되는 화면
    [SerializeField] private Canvas Canvas_Content;  // 인게임에서 한가지씩만 열리는 화면
    [SerializeField] private Canvas Canvas_Popup;  // 여러개 열러도 상관없는 팝업
    [SerializeField] private Canvas Canvas_Front;  // 로딩화면
    [SerializeField] private Canvas Canvas_ETC;

    [SerializeField] private LoadingOverLayView LoadingOverLay;

    // UI의 주소를 관리하는 딕셔너리
    private readonly Dictionary<UIType, string> _addressMap = new Dictionary<UIType, string>
    {
        {UIType.TitleUI, "UI_Title" },
        {UIType.CharacterSelectUI, "UI_CharacterSelect" },
        {UIType.InGameUI, "UI_InGame" },
        {UIType.ShopUI, "UI_Shop" },
        {UIType.HuntingAreaSelectUI, "UI_HuntingAreaSelect" },
        {UIType.InformationPopup, "Popup_Information" },
        {UIType.CharacterCreatePopup, "Popup_CharacterCreate" },
        {UIType.ConfirmPopup, "Popup_Confirm" },
        {UIType.GameMenuPopup, "Popup_GameMenu" },
        {UIType.InventoryPopup, "Popup_Inventory" },
        {UIType.SkillTreePopup, "Popup_SkillTree"},
        {UIType.EquipmentPopup, "Popup_Equipment" }
    };

    // UI가 배치될 레이어 관리
    private readonly Dictionary<UIType, UIRootType> _rootTypeMap = new Dictionary<UIType, UIRootType>
    {
        {UIType.TitleUI, UIRootType.Main },
        {UIType.CharacterSelectUI, UIRootType.Main },
        {UIType.InGameUI, UIRootType.Main },
        {UIType.HuntingAreaSelectUI, UIRootType.Content },
        {UIType.ShopUI, UIRootType.Content },
        {UIType.InformationPopup, UIRootType.Popup },
        {UIType.CharacterCreatePopup, UIRootType.Popup },
        {UIType.ConfirmPopup, UIRootType.Popup },
        {UIType.GameMenuPopup, UIRootType.Popup },
        {UIType.InventoryPopup, UIRootType.Popup },
        {UIType.SkillTreePopup, UIRootType.Content },
        {UIType.EquipmentPopup, UIRootType.Popup }
    };

    // UI가 중복으로 배치될지 한 레이어에 하나만 배치될지 bool값으로 관리
    private readonly Dictionary<UIRootType, bool> _exclusiveRootMap = new Dictionary<UIRootType, bool>
    {
        {UIRootType.BackGround, false },
        {UIRootType.Main, true },
        {UIRootType.Content, false },
        {UIRootType.Popup, false },
        {UIRootType.Front, false },
        {UIRootType.ETC, false }
    };

    // 생성된 UI 인스턴스 캐싱 + 현재 활성화된 UI 추적
    private readonly Dictionary<UIType, BaseUI> _uiDic = new Dictionary<UIType, BaseUI>();
    private readonly HashSet<UIType> _activeUI = new HashSet<UIType>();
    private readonly Dictionary<UIRootType, UIType> _activeUIByRoot = new Dictionary<UIRootType, UIType>();

    // UI를 열기 위해선 무조건 이 메서드를 통해서 열려야 함.
    public async UniTask<T> OpenUIAsync<T>(UIType uiType, bool useFullScreenLoading = false) where T : BaseUI
    {
        UIRootType rootType = GetRootType(uiType);
        bool needsLoad = _uiDic.ContainsKey(uiType) == false;
        bool shouldShowLoading = needsLoad && rootType != UIRootType.Popup;

        BaseUI ui;

        if (shouldShowLoading)
        {
            UniTask visualTask = LoadingOverLay.ShowAsync(useFullScreenLoading);
            UniTask<BaseUI> loadTask = GetOrCreateUIAsync(uiType);

            await visualTask;
            ui = await loadTask;

            LoadingOverLay.Hide();
        }
        else
        {
            ui = await GetOrCreateUIAsync(uiType);
        }

        if (ui == null)
        {
            return null;
        }

        CloseExclusiveUIIfNeeded(uiType, rootType);
        CloseAllExceptMainIfNeeded(rootType);

        ui.gameObject.SetActive(true);
        _activeUI.Add(uiType);
        _activeUIByRoot[rootType] = uiType;

        return ui as T;
    }

    public void CloseUI(UIType uiType)
    {
        if (_activeUI.Contains(uiType) == false) return;

        if (_activeUI.Contains(uiType) == false)
        {
            Debug.LogError($"[UIManager] {uiType}은 열려있지 않습니다.");
            return;
        }

        if (_uiDic.TryGetValue(uiType, out BaseUI ui) == false)
        {
            Debug.LogError($"[UIManager] {uiType}을 찾을 수 없습니다.");
            return;
        }

        ui.gameObject.SetActive(false);
        _activeUI.Remove(uiType);
    }

    public bool IsActiveUI(UIType uIType)
    {
        return _activeUI.Contains(uIType);
    }

    private void CloseExclusiveUIIfNeeded(UIType newUIType, UIRootType rootType)
    {
        if (_exclusiveRootMap.TryGetValue(rootType, out bool isExclusive) == false || isExclusive == false) return;
        if (_activeUIByRoot.TryGetValue(rootType, out UIType previousUIType) == false) return;
        if (previousUIType == newUIType) return;
        if (_activeUI.Contains(previousUIType)) CloseUI(previousUIType);
    }

    private void CloseAllExceptMainIfNeeded(UIRootType newUIRootType)
    {
        if (newUIRootType != UIRootType.Main)
        {
            return;
        }

        List<UIType> toClose = new List<UIType>();

        foreach (UIType activeType in _activeUI)
        {
            UIRootType activeRootType = GetRootType(activeType);

            if (activeRootType != UIRootType.Main)
            {
                toClose.Add(activeType);
            }
        }

        foreach (UIType type in toClose)
        {
            CloseUI(type);
        }
    }
    
    private UIRootType GetRootType(UIType uiType)
    {
        if (_rootTypeMap.TryGetValue(uiType, out UIRootType rootType) == false)
        {
            Debug.LogError($"[UIManager] {uiType}에 대한 UIRootType이 없습니다.");
            return UIRootType.None;
        }

        return rootType;
    }

    public T GetUI<T>(UIType uiType) where T : BaseUI
    {
        if (_uiDic.TryGetValue(uiType, out BaseUI ui))
        {
            return ui as T;
        }

        return null;
    }

    // 실제 로드/생성 로직 
    private async UniTask<BaseUI> GetOrCreateUIAsync(UIType uiType)
    {
        if (_uiDic.TryGetValue(uiType, out BaseUI cached))
        {
            return cached;
        }

        if (_addressMap.TryGetValue(uiType, out string address) == false)
        {
            Debug.LogError($"[UIManager] {uiType}에 대한 어드레서블 주소가 없습니다.");
            return null;
        }

        Canvas canvas = GetCanvas(uiType);

        if (canvas == null)
        {
            return null;
        }

        GameObject prefab = await LoadPrefabAsync(address);

        if (prefab == null)
        {
            return null;
        }


        GameObject instance = Instantiate(prefab, canvas.transform);
        instance.SetActive(false);

        BaseUI baseUI = instance.GetComponent<BaseUI>();

        if (baseUI == null)
        {
            Debug.LogError($"[UIManager] {instance.name}에 BaseUI 컴포넌트가 없습니다.");
            return null;
        }

        baseUI.SetUIType(uiType);

        _uiDic.Add(uiType, baseUI);

        return baseUI;

    }

    private async UniTask<GameObject> LoadPrefabAsync(string address)
    {
        return await Addressables.LoadAssetAsync<GameObject>(address).ToUniTask();
    }

    private Canvas GetCanvas(UIType uiType)
    {
        if (_rootTypeMap.TryGetValue(uiType, out UIRootType rootType) == false)
        {
            Debug.LogError($"[UIManager] {uiType}에 대한 UIRootType이 없습니다.");
            return null;
        }

        switch (rootType)
        {
            case UIRootType.BackGround: return Canvas_BackGround;
            case UIRootType.Main: return Canvas_Main;
            case UIRootType.Content: return Canvas_Content;
            case UIRootType.Popup: return Canvas_Popup;
            case UIRootType.Front: return Canvas_Front;
            case UIRootType.ETC: return Canvas_ETC;
            default:
                Debug.LogError($"[UIManager] {rootType}에 해당하는 Canvas가 없습니다.");
                return null;
        }
    }

    // 기능
    public bool HasActivePopup()
    {
        foreach(UIType type in _activeUI)
        {
            if (GetRootType(type) == UIRootType.Popup) return true;
        }

        return false;
    }

    public void CloseAllPopups()
    {
        List<UIType> toClose = new List<UIType>();

        foreach (UIType type in _activeUI)
        {
            if (GetRootType(type) == UIRootType.Popup) toClose.Add(type);
        }

        foreach (UIType type in toClose)
        {
            CloseUI(type);
        }
    }
}
