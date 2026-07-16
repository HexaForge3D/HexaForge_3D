using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

// 게임의 흐름 타이밍을 관리할 스크립트
public class GameFlowManager
{
    private string _currentSlotId;
    private string _pendingDeleteSlotId;
    private InGameViewModel _inGameViewModel;

    public async UniTask StartAsync()
    {
        await GameDataManager.Instance.WaitUntilReadyAsync();
        await ShowTitleAsync();
    }

    // 요청 메서드 모음
    private void OnGameStartRequested()
    {
        ShowCharacterSelectAsync().Forget();
    }

    private void OnEnterGameRequested(PlayerData data)
    {
        _currentSlotId = data.Id;
        ShowInGameAsync(data).Forget();
    }

    private void OnBackToCharacterSelectRequested()
    {
        PlayerInputSystem.OnInformation -= OnInformationKeyPressed;
        PlayerInputSystem.OnInventory -= OnInventoryKeyPressed;
        Portal.OnPortalInteracted -= OnPortalInteracted;
        PlayerInputSystem.OnSystem -= OnEscapeKeyPressed;
        PlayerBattle.OnHpChanged -= OnPlayerHpChanged;
        PlayerSpawnManager.Instance.DeSpawnPlayer();
        _currentSlotId = null;
        _inGameViewModel = null;
        ShowCharacterSelectAsync().Forget();
    }

    private void OnMenuCharacterSelectRequested()
    {
        UIManager.Instance.CloseUI(UIType.GameMenuPopup);
        OnBackToCharacterSelectRequested();
    }

    private void OnTeleportRequested(HuntingAreaData data)
    {
        MapManager.Instance.ChangeMap(data.MapName);
        UIManager.Instance.CloseUI(UIType.HuntingAreaSelectUI);
    }

    private void OnDeleteRequested(string slotId)
    {
        _pendingDeleteSlotId = slotId;
        ShowConfirmAsync("Delete this Character?", OnDeleteConfirmed).Forget();
    }

    private void OnQuitGameRequested()
    {
        ShowConfirmAsync("Quit the Game?", OnQuitGameConfirmed).Forget();
    }


    private void OnCreateCharacterRequested(string slotId)
    {
        ShowCharacterCreateAsync(slotId).Forget();
    }

    private void OnPortalInteracted(Portal portal)
    {
        var targetPortal = PortalManager.Instance.GetDestinationPortal(portal);
        if (portal.PortalType != PortalType.Dungeon)
        {
            if (portal.PortalType == PortalType.None) return;
            MapManager.Instance.TeleportToDestinationPortal(targetPortal);
        }
        else
        {
            ShowHuntingAreaAsync().Forget();
        }
    }

    private void OnCharacterCreated()
    {
        UIManager.Instance.CloseUI(UIType.CharacterCreatePopup);
        ShowCharacterSelectAsync().Forget();
    }

    private void OnDeleteConfirmed()
    {
        SaveManager.Instance.DeleteCharacter(_pendingDeleteSlotId);
        ShowCharacterSelectAsync().Forget();
    }

    private void OnQuitGameConfirmed()
    {
        Application.Quit();
    }

    private void OnPlayerHpChanged(int currentHp, int maxHp)
    {
        _inGameViewModel?.HandleHpChanged(currentHp, maxHp);
    }

    private void OnShopTransactionCompleted()
    {
        if (UIManager.Instance.IsActiveUI(UIType.InventoryPopup))
        {
            InventoryView inventoryView = UIManager.Instance.GetUI<InventoryView>(UIType.InventoryPopup);
            inventoryView?.Refresh();
        }
    }

    // 아직 미구독상태
    private void OnShopNpcInteracted()
    {
        ToggleUI(UIType.ShopUI, ShowShop);
    }

    private void OnInformationKeyPressed()
    {
        ToggleUI(UIType.InformationPopup, ShowInformation);
    }

    private void OnInventoryKeyPressed()
    {
        ToggleUI(UIType.InventoryPopup, ShowInventory); 
    }

    private void OnEscapeKeyPressed()
    {
        if (UIManager.Instance.HasActivePopup())
        {
            UIManager.Instance.CloseAllPopups();
        }
        else
        {
            ShowGameMenuAsync().Forget();
        }
    }


    // 요청 수행 메서드 모음
    private async UniTask ShowTitleAsync()
    {
        TitleView view = await UIManager.Instance.OpenUIAsync<TitleView>(UIType.TitleUI);

        TitleViewModel viewModel = new TitleViewModel();
        viewModel.OnGameStartRequested += OnGameStartRequested;

        view.BindViewModel(viewModel);
    }

    private async UniTask ShowCharacterSelectAsync()
    {
        CharacterSelectView view = await UIManager.Instance.OpenUIAsync<CharacterSelectView>(UIType.CharacterSelectUI, useFullScreenLoading: false);

        CharacterSelectViewModel viewModel = new CharacterSelectViewModel();
        viewModel.OnEnterGameRequested += OnEnterGameRequested;
        viewModel.OnCreateCharacterRequested += OnCreateCharacterRequested;
        viewModel.OnDeleteRequested += OnDeleteRequested;

        view.BindViewModel(viewModel);
    }

    private async UniTask ShowInGameAsync(PlayerData data)
    {
        PlayerTableData jobMaster = GameDataManager.Instance.GetData<PlayerTableData>(data.Job);
        
        if (jobMaster == null)
        {
            Debug.LogError($"[GameFlowManager] {data.Job}에 대한 직업 마스터 데이터를 찾을 수 없습니다.");
            return;
        }

        await PlayerSpawnManager.Instance.SpawnPlayerAsync(data, jobMaster.PrefabAddress);

        InGameView view = await UIManager.Instance.OpenUIAsync<InGameView>(UIType.InGameUI, useFullScreenLoading: true);

        _inGameViewModel = new InGameViewModel();

        view.BindViewModel(_inGameViewModel);

        PlayerInputSystem.OnInformation += OnInformationKeyPressed;
        PlayerInputSystem.OnInventory += OnInventoryKeyPressed;
        Portal.OnPortalInteracted += OnPortalInteracted;
        PlayerInputSystem.OnSystem += OnEscapeKeyPressed;
        PlayerBattle.OnHpChanged += OnPlayerHpChanged;

        ShowShop();
    }

    private async UniTask ShowHuntingAreaAsync()
    {
        HuntingAreaSelectView view = await UIManager.Instance.OpenUIAsync<HuntingAreaSelectView>(UIType.HuntingAreaSelectUI, useFullScreenLoading: false);

        HuntingAreaSelectViewModel viewModel = new HuntingAreaSelectViewModel();
        viewModel.OnTeleportRequested += OnTeleportRequested;

        view.BindViewModel(viewModel);
    }

    private async UniTask ShowInformationAsync()
    {
        InformationView view = await UIManager.Instance.OpenUIAsync<InformationView>(UIType.InformationPopup);

        InformationViewModel viewModel = new InformationViewModel(_currentSlotId);
        
        view.BindViewModel(viewModel);

    }

    private async UniTask ShowCharacterCreateAsync(string slotId)
    {
        CharacterCreateView view  = await UIManager.Instance.OpenUIAsync<CharacterCreateView>(UIType.CharacterCreatePopup);
        CharacterCreateViewModel viewModel = new CharacterCreateViewModel(slotId);
        viewModel.OnCharacterCreated += OnCharacterCreated;

        view.BindViewModel(viewModel);
    }

    private async UniTask ShowConfirmAsync(string message, Action onConfirmed)
    {
        ConfirmView view = await UIManager.Instance.OpenUIAsync<ConfirmView>(UIType.ConfirmPopup);
        ConfirmViewModel viewModel = new ConfirmViewModel(message, onConfirmed);
        view.BindViewModel(viewModel);
    }

    private async UniTask ShowGameMenuAsync()
    {
        GameMenuView view = await UIManager.Instance.OpenUIAsync<GameMenuView>(UIType.GameMenuPopup);
        GameMenuViewModel viewModel = new GameMenuViewModel();
        viewModel.OnBackToCharacterSelectRequested += OnMenuCharacterSelectRequested;
        viewModel.OnQuitGameRequested += OnQuitGameRequested;

        view.BindViewModel(viewModel);
    }

    private async UniTask ShowInventoryAsync()
    {
        InventoryView view = await UIManager.Instance.OpenUIAsync<InventoryView>(UIType.InventoryPopup);
        InventoryViewModel viewModel = new InventoryViewModel(_currentSlotId);
        view.BindViewModel(viewModel);
    }

    private async UniTask ShowShopAsync()
    {
        ShopView view = await UIManager.Instance.OpenUIAsync<ShopView>(UIType.ShopUI);
        ShopViewModel viewModel = new ShopViewModel(_currentSlotId);
        viewModel.OnGoldChanged += OnShopTransactionCompleted;
        view.BindViewModel(viewModel);
    }


    // UI 토글화
    private void ToggleUI(UIType uiType, Action showAction)
    {
        if (UIManager.Instance.IsActiveUI(uiType))
        {
            UIManager.Instance.CloseUI(uiType);
        }
        else
        {
            showAction();
        }
    }

    private void ShowInformation()
    {
        ShowInformationAsync().Forget();
    }

    private void ShowInventory()
    {
        ShowInventoryAsync().Forget();
    }

    private void ShowShop()
    {
        ShowShopAsync().Forget();
    }
}
