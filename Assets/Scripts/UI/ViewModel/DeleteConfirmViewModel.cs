using System;
using UnityEngine;

public class DeleteConfirmViewModel
{
    private readonly string _slotId;

    public Action OnConfirmed;

    public DeleteConfirmViewModel(string slotId)
    {
        _slotId = slotId;
    }

    public void RequestConfirm()
    {
        bool success = SaveManager.Instance.DeleteCharacter(_slotId);

        if (success)
        {
            OnConfirmed?.Invoke();
        }
    }
}
