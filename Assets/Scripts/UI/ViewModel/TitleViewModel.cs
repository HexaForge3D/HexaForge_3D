using System;
using UnityEngine;

public class TitleViewModel
{
    public Action OnGameStartRequest;

    public void RequestGameStart()
    {
        OnGameStartRequest?.Invoke();
    }
}
