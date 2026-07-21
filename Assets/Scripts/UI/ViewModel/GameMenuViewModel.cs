using System;

public class GameMenuViewModel
{
    public Action OnBackToCharacterSelectRequested;
    public Action OnQuitGameRequested;
    public Action OnSettingsRequested;

    public void RequestBackToCharacterSelect()
    {
        OnBackToCharacterSelectRequested?.Invoke();
    }

    public void RequestQuitGame()
    {
        OnQuitGameRequested?.Invoke();
    }

    public void RequestSettings()
    {
        OnSettingsRequested?.Invoke();
    }
}
