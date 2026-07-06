using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectView : BaseUI       
{
    [SerializeField] private Button Button_EnterGame;

    private CharacterSelectViewModel _viewModel;

    public void BindViewModel(CharacterSelectViewModel viewModel)
    {
        _viewModel = viewModel;

        Button_EnterGame.onClick.RemoveAllListeners();
        Button_EnterGame.onClick.AddListener(OnClickEnterGame);
    }

    private void OnClickEnterGame()
    {
        _viewModel?.RequestEnterGame();
    }
}
