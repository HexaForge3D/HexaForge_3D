using System;

public class ConfirmViewModel
{
    public readonly Action _onConfirmed;

    public string Message { get; }

    public ConfirmViewModel(string message, Action onConfirmed)
    {
        Message = message;
        _onConfirmed = onConfirmed;
    }

    public void RequestConfirm()
    {
        _onConfirmed?.Invoke();
    }
}
