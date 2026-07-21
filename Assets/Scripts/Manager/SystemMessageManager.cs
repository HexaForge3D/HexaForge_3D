using UnityEngine;

public class SystemMessageManager : BaseMonoManager<SystemMessageManager>
{
    [SerializeField] private SystemMessageView MessageView;

    public void Show(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        MessageView.Show(message);
    }
}