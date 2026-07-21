using System;

public class GameMenuViewModel
{
    public Action OnBackToCharacterSelectRequested;
    public Action OnQuitGameRequested;

    public void RequestBackToCharacterSelect()
    {
        OnBackToCharacterSelectRequested?.Invoke();
    }

    public void RequestQuitGame()
    {
        OnQuitGameRequested?.Invoke();
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
