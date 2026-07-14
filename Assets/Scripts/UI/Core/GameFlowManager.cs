using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

// 게임의 흐름 타이밍을 관리할 스크립트
public class GameFlowManager
{
    private string _currentSlotId;
    private string _pendingDeleteSlotId;

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
        Portal.OnPortalInteracted -= OnPortalInteracted;
        PlayerInputSystem.OnSystem -= OnEscapeKeyPressed;
        PlayerSpawnManager.Instance.DeSpawnPlayer();
        _currentSlotId = null;
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

    private void OnPortalInteracted(Portal portal)
    {
        switch (portal.PortalType)
        {
            case PortalType.Dungeon:
                {
                    ShowHuntingAreaAsync().Forget();
                }
                break;
            case PortalType.MainQuest:
                {
                    Debug.Log("MainQuest 이동");
                }
                break;
            case PortalType.Smithy:
                {
                    Debug.Log("Smithy 이동");
                }
                break;
            case PortalType.Store:
                {
                    Debug.Log("Store 이동");
                }
                break;
            default:
                {
                    Debug.Log("PortalType 설정이 잘못되었습니다.");
                }
                break;
        }
    }

    private void OnInformationKeyPressed()
    {
        ShowInformationAsync().Forget();
    }
    
    private void OnCreateCharacterRequested(string slotId)
    {
        ShowCharacterCreateAsync(slotId).Forget();
    }

    private void OnCharacterCreated()
    {
        UIManager.Instance.CloseUI(UIType.CharacterCreatePopup);
        ShowCharacterSelectAsync().Forget();
    }

    private void OnDeleteRequested(string slotId)
    {
        _pendingDeleteSlotId = slotId;
        ShowConfirmAsync("Delete this Character?", OnDeleteConfirmed).Forget();
    }

    private void OnDeleteConfirmed()
    {
        SaveManager.Instance.DeleteCharacter(_pendingDeleteSlotId);
        ShowCharacterSelectAsync().Forget();
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

    private void OnQuitGameRequested()
    {
        ShowConfirmAsync("Quit the Game?", OnQuitGameConfirmed).Forget();
    }

    private void OnQuitGameConfirmed()
    {
        Application.Quit();
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

        InGameViewModel viewModel = new InGameViewModel();

        view.BindViewModel(viewModel);

        PlayerInputSystem.OnInformation += OnInformationKeyPressed;
        Portal.OnPortalInteracted += OnPortalInteracted;
        PlayerInputSystem.OnSystem += OnEscapeKeyPressed;
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
        InformationView view = await UIManager.Instance.OpenUIAsync<InformationView>(UIType.InformationUI);

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
}
