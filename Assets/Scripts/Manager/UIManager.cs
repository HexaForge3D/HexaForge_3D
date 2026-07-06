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
    [SerializeField] private Canvas Canvas_Main;
    [SerializeField] private Canvas Canvas_Content;
    [SerializeField] private Canvas Canvas_Popup;
    [SerializeField] private Canvas Canvas_Front;
    [SerializeField] private Canvas Canvas_ETC;

    private readonly Dictionary<UIType, string> _addressMap = new Dictionary<UIType, string>
    {
        {UIType.TitleUI, "UI_Title" },
        {UIType.CharacterSelectUI, "UI_CharacterSelect" },
        {UIType.InGameUI, "UI_InGame" },
        {UIType.HuntingAreaSelectUI, "UI_HuntingAreaSelect" },
    };

    private readonly Dictionary<UIType, UIRootType> _rootTypeMap = new Dictionary<UIType, UIRootType>
    {
        {UIType.TitleUI, UIRootType.Main },
        {UIType.CharacterSelectUI, UIRootType.Main },
        {UIType.InGameUI, UIRootType.Main },
        {UIType.HuntingAreaSelectUI, UIRootType.Popup },
    };

    private readonly Dictionary<UIType, BaseUI> _uiDic = new Dictionary<UIType, BaseUI>();

    private readonly HashSet<UIType> _activeUI = new HashSet<UIType>();

    public async UniTask<T> OpenUIAsync<T>(UIType uiType) where T : BaseUI
    {
        BaseUI ui = await GetOrCreateUIAsync(uiType);

        if (ui == null)
        {
            return null;
        }

        ui.gameObject.SetActive(true);
        _activeUI.Add(uiType);

        return ui as T;
    }

    public void CloseUI(UIType uiType)
    {
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
}
