using System;

public class GameMenuViewModel
{
    public Action OnBackToCharacterSelectRequested;
    public Action OnSettingsRequested;
    public Action OnHelpRequested;
    public Action OnQuitGameRequested;

    public void RequestBackToCharacterSelect()
    {
        OnBackToCharacterSelectRequested?.Invoke();
    }

    public void RequestSettings()
    {
        OnSettingsRequested?.Invoke();
    }

    public void RequestHelp()
    {
        OnHelpRequested?.Invoke();
    }

    public void RequestQuitGame()
    {
        OnQuitGameRequested?.Invoke();
    }
}
