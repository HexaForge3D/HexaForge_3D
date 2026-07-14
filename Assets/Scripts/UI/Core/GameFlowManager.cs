using Cysharp.Threading.Tasks;
using UnityEngine;

// 게임의 흐름 타이밍을 관리할 스크립트
public class GameFlowManager
{
    private string _currentSlotId;

    public async UniTask StartAsync()
    {
        await GameDataManager.Instance.WaitUntilReadyAsync();
        await ShowTitleAsync();
    }

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
        PlayerSpawnManager.Instance.DeSpawnPlayer();
        _currentSlotId = null;
        ShowCharacterSelectAsync().Forget();
    }

    private void OnHuntingAreaSelectRequested()
    {
        ShowHuntingAreaAsync().Forget();
    }

    private void OnTeleportRequest(HuntingAreaData data)
    {
        MapManager.Instance.ChangeMap(data.MapName);
        UIManager.Instance.CloseUI(UIType.HuntingAreaSelectUI);
    }

    private void OnPortalInteracted(Portal portal)
    {
        ShowHuntingAreaAsync().Forget();
    }

    private void OnInformationKeyPressed()
    {
        ShowInformationAsync().Forget();
    }
    
    private void OnCreateCharacterRequest(string slotId)
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
        ShowDeleteConfirmAsync(slotId).Forget();
    }

    private void OnDeleteConfirmed()
    {
        UIManager.Instance.CloseUI(UIType.DeleteConfirmPopup);
        ShowCharacterSelectAsync().Forget();
    }


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
        viewModel.OnCreateCharacterRequested += OnCreateCharacterRequest;
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
        viewModel.OnBackToCharacterSelectRequested += OnBackToCharacterSelectRequested;
        viewModel.OnHuntingAreaSelectRequested += OnHuntingAreaSelectRequested;

        view.BindViewModel(viewModel);

        PlayerInputSystem.OnInformation += OnInformationKeyPressed;
        Portal.OnPortalInteracted += OnPortalInteracted;
    }

    private async UniTask ShowHuntingAreaAsync()
    {
        HuntingAreaSelectView view = await UIManager.Instance.OpenUIAsync<HuntingAreaSelectView>(UIType.HuntingAreaSelectUI, useFullScreenLoading: false);

        HuntingAreaSelectViewModel viewModel = new HuntingAreaSelectViewModel();
        viewModel.OnTeleportRequested += OnTeleportRequest;

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

    private async UniTask ShowDeleteConfirmAsync(string slotId)
    {
        DeleteConfirmView view = await UIManager.Instance.OpenUIAsync<DeleteConfirmView>(UIType.DeleteConfirmPopup);
        DeleteConfirmViewModel viewModel = new DeleteConfirmViewModel(slotId);
        viewModel.OnConfirmed += OnDeleteConfirmed;
        view.BindViewModel(viewModel);
    }
}
