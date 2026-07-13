using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class DeleteConfirmView : BaseOverLayUI
{
    [SerializeField] private TMP_Text Text_Message;
    [SerializeField] private Button Button_Confirm;

    private DeleteConfirmViewModel _viewModel;

    public void BindViewModel(DeleteConfirmViewModel viewModel)
    {
        _viewModel = viewModel;

        Text_Message.text = "Delete this Character?";

        Button_Confirm.onClick.RemoveAllListeners();
        Button_Confirm.onClick.AddListener(OnClickConfirm);
    }

    private void OnClickConfirm()
    {
        _viewModel?.RequestConfirm();
    }
}
