using UnityEngine.UI;
using UnityEngine;

public class GameMenuView : BaseOverLayUI
{
    [SerializeField] private Button Button_CharacterSelect;
    [SerializeField] private Button Button_QuitGame;

    [Header("Volume Sliders")]
    [SerializeField] private Slider Slider_BGM;
    [SerializeField] private Slider Slider_SFX;
    [SerializeField] private Slider Slider_UI;

    private GameMenuViewModel _viewModel;

    public void BindViewModel(GameMenuViewModel viewModel)
    {
        _viewModel = viewModel;

        Button_CharacterSelect.onClick.RemoveListener(OnClickCharacterSelect);
        Button_CharacterSelect.onClick.AddListener(OnClickCharacterSelect);

        Button_QuitGame.onClick.RemoveListener(OnClickQuitGame);
        Button_QuitGame.onClick.AddListener(OnClickQuitGame);


        if (Slider_BGM != null)
        {
            Slider_BGM.onValueChanged.RemoveListener(OnChangeBGM);
            Slider_BGM.onValueChanged.AddListener(OnChangeBGM);
        }
        if (Slider_SFX != null)
        {
            Slider_SFX.onValueChanged.RemoveListener(OnChangeSFX);
            Slider_SFX.onValueChanged.AddListener(OnChangeSFX);
        }
        if (Slider_UI != null)
        {
            Slider_UI.onValueChanged.RemoveListener(OnChangeUI);
            Slider_UI.onValueChanged.AddListener(OnChangeUI);
        }
    }

    private void OnClickCharacterSelect()
    {
        _viewModel?.RequestBackToCharacterSelect();
    }

    private void OnClickQuitGame()
    {
        _viewModel?.RequestQuitGame();
    }

    private void OnChangeBGM(float value)
    {
        _viewModel?.ChangeBGMVolume(value);
    }

    private void OnChangeSFX(float value)
    {
        _viewModel?.ChangeSFXVolume(value);
    }

    private void OnChangeUI(float value)
    {
        _viewModel?.ChangeUIVolume(value);
    }
}
