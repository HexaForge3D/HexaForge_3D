using UnityEngine.UI;
using UnityEngine;

public class GameMenuView : BaseOverLayUI
{
    [SerializeField] private Button Button_CharacterSelect;
    [SerializeField] private Button Button_QuitGame;
    [SerializeField] private Button Button_Settings;

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
        Debug.Log("[GameMenuView] OnClickSettings 호출됨");
        _viewModel?.RequestSettings();
    }
}
