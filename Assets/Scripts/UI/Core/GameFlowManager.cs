using Cysharp.Threading.Tasks;

// 게임의 흐름 타이밍을 관리할 스크립트
public class GameFlowManager
{
    public async UniTask StartAsync()
    {
        await ShowTitleAsync();
    }

    private async UniTask ShowTitleAsync()
    {
        TitleView view = await UIManager.Instance.OpenUIAsync<TitleView>(UIType.TitleUI);

        TitleViewModel viewModel = new TitleViewModel();
        viewModel.OnGameStartRequest += OnGameStartRequest;

        view.BindViewModel(viewModel);
    }

    private void OnGameStartRequest()
    {
        ShowCharacterSelectAsync().Forget();
    }

    private async UniTask ShowCharacterSelectAsync()
    {
        CharacterSelectView view = await UIManager.Instance.OpenUIAsync<CharacterSelectView>(UIType.CharacterSelectUI);

        CharacterSelectViewModel viewModel = new CharacterSelectViewModel();
        viewModel.OnEnterGameRequested += OnEnterGameRequest;

        view.BindViewModel(viewModel);
    }

    private void OnEnterGameRequest()
    {

    }
}
