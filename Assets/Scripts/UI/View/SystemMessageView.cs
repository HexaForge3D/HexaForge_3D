using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SystemMessageView : MonoBehaviour
{
    [SerializeField] private TMP_Text Text_Message;
    [SerializeField] private CanvasGroup CanvasGroup_Self;
    [SerializeField] private float DisplayDuration = 2f;

    private int _showToken;

    private void Awake()
    {
        CanvasGroup_Self.alpha = 0f;
        CanvasGroup_Self.blocksRaycasts = false; 
        CanvasGroup_Self.interactable = false;   
    }

    public void Show(string message)
    {
        Debug.Log($"[SystemMessageView] Show 호출됨: '{message}', activeInHierarchy: {gameObject.activeInHierarchy}");
        ShowAsync(message).Forget();
    }

    private async UniTask ShowAsync(string message)
    {
        int myToken = ++_showToken;

        Text_Message.text = message;
        CanvasGroup_Self.alpha = 1f;

        await UniTask.Delay((int)(DisplayDuration * 1000));

        if (myToken == _showToken)
        {
            CanvasGroup_Self.alpha = 0f;
        }
    }
}