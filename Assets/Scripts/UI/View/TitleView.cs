using UnityEngine;
using UnityEngine.UI;

public class TitleView : BaseUI
{
    [SerializeField] private Button Button_GameStart;

    private TitleViewModel _viewModel;

    private void Start()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGMSound("TitleBGM");
        }
    }
    public void BindViewModel(TitleViewModel viewModel)
    {
        _viewModel = viewModel;

        Button_GameStart.onClick.RemoveListener(OnClickGameStart);
        Button_GameStart.onClick.AddListener(OnClickGameStart);
    }

    private void OnClickGameStart()
    {
        _viewModel?.RequestGameStart();
    }
}
