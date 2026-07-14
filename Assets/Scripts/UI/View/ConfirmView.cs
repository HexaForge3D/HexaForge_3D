using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ConfirmView : BaseOverLayUI
{
    [SerializeField] private TMP_Text Text_Message;
    [SerializeField] private Button Button_Confirm;

    private ConfirmViewModel _viewModel;

    public void BindViewModel(ConfirmViewModel viewModel)
    {
        _viewModel = viewModel;

        Text_Message.text = viewModel.Message;

        Button_Confirm.onClick.RemoveAllListeners();
        Button_Confirm.onClick.AddListener(OnClickConfirm);
    }

    private void OnClickConfirm()
    {
        _viewModel?.RequestConfirm();
        UIManager.Instance.CloseUI(UIType.ConfirmPopup);
    }
}
