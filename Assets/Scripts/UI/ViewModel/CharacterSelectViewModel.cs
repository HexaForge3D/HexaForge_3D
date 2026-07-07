using System;
using UnityEngine;

public class CharacterSelectViewModel
{
    public Action OnEnterGameRequested;

    public void RequestEnterGame()
    {
        OnEnterGameRequested?.Invoke();
    }
}
