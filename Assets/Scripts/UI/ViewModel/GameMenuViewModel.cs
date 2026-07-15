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
}
