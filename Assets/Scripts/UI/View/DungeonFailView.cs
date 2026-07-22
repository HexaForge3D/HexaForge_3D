using System;
using UnityEngine;
using UnityEngine.UI;

public class DungeonFailView : BaseUI
{
    [SerializeField] private Button Button_ReturnToVillage;

    private Action _onReturnClicked;

    public void Setup(Action onReturnClicked)
    {
        _onReturnClicked = onReturnClicked;

        Button_ReturnToVillage.onClick.RemoveAllListeners();
        Button_ReturnToVillage.onClick.AddListener(OnClickReturn);
    }

    private void OnClickReturn()
    {
        _onReturnClicked?.Invoke();
    }
}