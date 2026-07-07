using UnityEngine;
using UnityEngine.UI;

public class BaseOverLayUI : BaseUI
{
    [SerializeField] private Button Button_Close;

    protected virtual void Awake()
    {
        Button_Close.onClick.AddListener(OnClickClose);
    }

    private void OnClickClose()
    {
        UIManager.Instance.CloseUI(UIType_This);
    }
}
