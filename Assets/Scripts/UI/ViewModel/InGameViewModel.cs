using System;
using UnityEngine;

public class InGameViewModel
{
    public Action OnBackToCharacterSelectRequest;

    public void RequestBackToCharacterSelect()
    {
        OnBackToCharacterSelectRequest?.Invoke();
    }
}
