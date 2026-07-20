using UnityEngine.UI;
using UnityEngine;
using System;

public class DeathView : BaseUI
{
    [SerializeField] private Button Button_Revive;

    private Action _onReviveClicked;

    public void Setup(Action onReviveClicked)
    {
        _onReviveClicked = onReviveClicked;

        Button_Revive.onClick.RemoveAllListeners();
        Button_Revive.onClick.AddListener(OnClickRevive);
    }

    private void OnClickRevive()
    {
        _onReviveClicked?.Invoke();
    }
}
