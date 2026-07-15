using System;
using UnityEngine;

public class TitleViewModel
{
    public Action OnGameStartRequested;

    public void RequestGameStart()
    {
        OnGameStartRequested?.Invoke();
    }
}
