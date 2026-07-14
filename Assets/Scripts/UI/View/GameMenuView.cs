using UnityEngine.UI;
using UnityEngine;

public class GameMenuView : BaseOverLayUI
{
    [SerializeField] private Button Button_CharacterSelect;
    [SerializeField] private Button Button_QuitGame;

    private GameMenuViewModel _viewModel;

    public void BindViewModel(GameMenuViewModel viewModel)
    {
        _viewModel = viewModel;

        Button_CharacterSelect.onClick.RemoveAllListeners();
        Button_CharacterSelect.onClick.AddListener(OnClickCharacterSelect);

        Button_QuitGame.onClick.RemoveAllListeners();
        Button_QuitGame.onClick.AddListener(OnClickQuitGame);
    }

    private void OnClickCharacterSelect()
    {
        _viewModel?.RequestBackToCharacterSelect();
    }

    private void OnClickQuitGame()
    {
        _viewModel?.RequestQuitGame();
    }
}
