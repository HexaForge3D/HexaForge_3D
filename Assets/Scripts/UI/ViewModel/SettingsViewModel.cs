using System.Collections.Generic;
using UnityEngine;

public class SettingsViewModel
{
    public List<string> GetResolutionOptions()
    {
        List<string> options = new List<string>();
        Resolution[] resolutions = GameSettingsManager.Instance.GetAvailableResolutions();

        foreach (Resolution resolution in resolutions)
        {
            options.Add($"{resolution.width} x {resolution.height}");
        }

        return options;
    }

    public int GetCurrentResolutionIndex()
    {
        return GameSettingsManager.Instance.GetCurrentResolutionIndex();
    }

    public bool GetIsFullscreen()
    {
        return GameSettingsManager.Instance.GetIsFullscreen();
    }

    public void ApplyResolution(int index)
    {
        bool isFullscreen = GameSettingsManager.Instance.GetIsFullscreen();
        GameSettingsManager.Instance.SetResolution(index, isFullscreen);
    }

    public void ApplyFullscreen(bool isFullscreen)
    {
        int currentIndex = GameSettingsManager.Instance.GetCurrentResolutionIndex();
        GameSettingsManager.Instance.SetResolution(currentIndex, isFullscreen);
    }

    public void ChangeBGMVolume(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
    }

    public void ChangeSFXVolume(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }

    public void ChangeUIVolume(float value)
    {
        SoundManager.Instance.SetUIVolume(value);
    }
}
