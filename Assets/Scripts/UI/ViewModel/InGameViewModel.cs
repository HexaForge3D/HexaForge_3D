using System;
using UnityEngine;

public class InGameViewModel
{
    public Action OnBackToCharacterSelectRequested;
    public Action OnHuntingAreaSelectRequested;

    public void RequestBackToCharacterSelect()
    {
        OnBackToCharacterSelectRequested?.Invoke();
    }

    public void RequestHuntingAreaSelect()
    {
        OnHuntingAreaSelectRequested?.Invoke();
    }
}
