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

    }
}
