using UnityEngine;

public class GameSettingsManager : BaseMonoManager<GameSettingsManager>
{
    private const string ResolutionIndexKey = "ResolutionIndex";
    private const string IsFullscreenKey = "IsFullscrren";

    private Resolution[] _availableResolutions;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == this)
        {
            _availableResolutions = Screen.resolutions;
            ApplySavedSettings();
        }
    }

    private void ApplySavedSettings()
    {
        int resolutionIndex = PlayerPrefs.GetInt(ResolutionIndexKey, _availableResolutions.Length - 1);
        bool isFullscreen = PlayerPrefs.GetInt(IsFullscreenKey, 1) == 1;

        SetResolution(resolutionIndex, isFullscreen);
    }

    public Resolution[] GetAvailableResolutions()
    {
        return _availableResolutions;
    }

    public int GetCurrentResolutionIndex()
    {
        return PlayerPrefs.GetInt(ResolutionIndexKey, _availableResolutions.Length - 1);
    }

    public bool GetIsFullscreen()
    {
        return PlayerPrefs.GetInt(IsFullscreenKey, 1) == 1;
    }

    public void SetResolution(int index, bool isFullscreen)
    {
        if (index < 0 || index >= _availableResolutions.Length) return;

        Resolution resolution = _availableResolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, isFullscreen);

        PlayerPrefs.SetInt(ResolutionIndexKey, index);
        PlayerPrefs.SetInt(IsFullscreenKey, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
