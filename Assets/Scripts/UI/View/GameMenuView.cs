using UnityEngine.UI;
using UnityEngine;

public class GameMenuView : BaseOverLayUI
{
    [SerializeField] private Button Button_CharacterSelect;
    [SerializeField] private Button Button_Settings;
    [SerializeField] private Button Button_Help;
    [SerializeField] private Button Button_QuitGame;

    private GameMenuViewModel _viewModel;

    public void BindViewModel(GameMenuViewModel viewModel)
    {
        _viewModel = viewModel;

        Button_CharacterSelect.onClick.RemoveListener(OnClickCharacterSelect);
        Button_CharacterSelect.onClick.AddListener(OnClickCharacterSelect);

        Button_QuitGame.onClick.RemoveListener(OnClickQuitGame);
        Button_QuitGame.onClick.AddListener(OnClickQuitGame);

        Button_Settings.onClick.RemoveListener(OnClickSettings);
        Button_Settings.onClick.AddListener(OnClickSettings);

        Button_Help.onClick.RemoveListener(OnClickHelp);
        Button_Help.onClick.AddListener(OnClickHelp);
    }

    private void OnClickCharacterSelect()
    {
        _viewModel?.RequestBackToCharacterSelect();
    }

    private void OnClickQuitGame()
    {
        _viewModel?.RequestQuitGame();
    }

    private void OnClickSettings()
    {
        _viewModel?.RequestSettings();
    }

    private void OnClickHelp()
    {
        _viewModel?.RequestHelp();
    }
}
