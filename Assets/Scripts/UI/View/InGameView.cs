using UnityEngine;
using UnityEngine.UI;

public class InGameView : BaseUI
{
    [SerializeField] private Button Button_BackToCharacterSelect;

    private InGameViewModel _viewModel;

    public void BindViewModel(InGameViewModel viewModel)
    {
        _viewModel = viewModel;

        Button_BackToCharacterSelect.onClick.RemoveAllListeners();
        Button_BackToCharacterSelect.onClick.AddListener(OnClickBackToCharacterSelect);
    }

    private void OnClickBackToCharacterSelect()
    {
        _viewModel?.RequestBackToCharacterSelect();
    }
}
