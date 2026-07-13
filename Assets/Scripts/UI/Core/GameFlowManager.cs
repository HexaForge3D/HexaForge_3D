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

}
