using Cysharp.Threading.Tasks;

// 게임의 흐름 타이밍을 관리할 스크립트
public class GameFlowManager
{
    public async UniTask StartAsync()
    {
        await ShowTitleAsync();
    }

    private void OnGameStartRequested()
    {
        ShowCharacterSelectAsync().Forget();
    }

    private void OnEnterGameRequested(PlayerData data)
    {
        ShowInGameAsync(data).Forget();
    }

    private void OnBackToCharacterSelectRequested()
    {
        PlayerInputSystem.OnInformation -= OnInformationKeyPressed;
        PlayerSpawnManager.Instance.DeSpawnPlayer();
        ShowCharacterSelectAsync().Forget();
    }

    private void OnHuntingAreaSelectRequested()
    {
        ShowHuntingAreaAsync().Forget();
    }

    private void OnTeleportRequest(HuntingAreaData data)
    {
        UnityEngine.Debug.Log($"텔레포트 요청: {data.Name} (Id: {data.Id})");
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
        await PlayerSpawnManager.Instance.SpawnPlayerAsync(data);

        InGameView view = await UIManager.Instance.OpenUIAsync<InGameView>(UIType.InGameUI, useFullScreenLoading: true);

        InGameViewModel viewModel = new InGameViewModel();
        viewModel.OnBackToCharacterSelectRequested += OnBackToCharacterSelectRequested;
        viewModel.OnHuntingAreaSelectRequested += OnHuntingAreaSelectRequested;

        view.BindViewModel(viewModel);

        PlayerInputSystem.OnInformation += OnInformationKeyPressed;
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

        InformationViewModel viewModel = new InformationViewModel();
        
        view.BindViewModel(viewModel);

    }

}
