using System;

public class ConfirmViewModel
{
    public readonly Action _onConfirmed;

    public string Message { get; }

    public string ConfirmUISoundName { get; }
    public ConfirmViewModel(string message, Action onConfirmed, string confirmUISoundName)
    {
        Message = message;
        _onConfirmed = onConfirmed;

        ConfirmUISoundName = confirmUISoundName;
    }

    public void RequestConfirm()
    {
        _onConfirmed?.Invoke();
    }
}
