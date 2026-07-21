using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SettingView : BaseOverLayUI
{
    [SerializeField] private TMP_Dropdown Dropdown_Resolution;
    [SerializeField] private Toggle Toggle_Fullscreen;

    [Header("Volume Sliders")]
    [SerializeField] private Slider Slider_BGM;
    [SerializeField] private Slider Slider_SFX;
    [SerializeField] private Slider Slider_UI;

    private SettingsViewModel _viewModel;

    public void BindViewModel(SettingsViewModel viewModel)
    {
        _viewModel = viewModel;

        List<string> options = viewModel.GetResolutionOptions();

        Dropdown_Resolution.ClearOptions();
        Dropdown_Resolution.AddOptions(options);
        Dropdown_Resolution.value = viewModel.GetCurrentResolutionIndex();
        Dropdown_Resolution.onValueChanged.RemoveListener(OnResolutionChanged);
        Dropdown_Resolution.onValueChanged.AddListener(OnResolutionChanged);

        Toggle_Fullscreen.isOn = viewModel.GetIsFullscreen();
        Toggle_Fullscreen.onValueChanged.RemoveListener(OnFullscreenChanged);
        Toggle_Fullscreen.onValueChanged.AddListener(OnFullscreenChanged);

        Slider_BGM.onValueChanged.RemoveListener(OnChangeBGM);
        Slider_BGM.onValueChanged.AddListener(OnChangeBGM);

        Slider_SFX.onValueChanged.RemoveListener(OnChangeSFX);
        Slider_SFX.onValueChanged.AddListener(OnChangeSFX);

        Slider_UI.onValueChanged.RemoveListener(OnChangeUI);
        Slider_UI.onValueChanged.AddListener(OnChangeUI);
    }

    private void OnResolutionChanged(int index)
    {
        _viewModel.ApplyResolution(index);
    }

    private void OnFullscreenChanged(bool isFullscreen)
    {
        _viewModel.ApplyFullscreen(isFullscreen);
    }

    private void OnChangeBGM(float value)
    {
        _viewModel.ChangeBGMVolume(value);
    }

    private void OnChangeSFX(float value)
    {
        _viewModel.ChangeSFXVolume(value);
    }

    private void OnChangeUI(float value)
    {
        _viewModel.ChangeUIVolume(value);
    }
}
