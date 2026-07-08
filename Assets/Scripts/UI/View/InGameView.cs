using UnityEngine;
using UnityEngine.UI;

public class InGameView : BaseUI
{
    [SerializeField] private Button Button_BackToCharacterSelect;
    [SerializeField] private Button Button_HuntingAreaSelect;

    private InGameViewModel _viewModel;

    public void BindViewModel(InGameViewModel viewModel)
    {
        _viewModel = viewModel;

        Button_BackToCharacterSelect.onClick.RemoveAllListeners();
        Button_BackToCharacterSelect.onClick.AddListener(OnClickBackToCharacterSelect);

        Button_HuntingAreaSelect.onClick.RemoveAllListeners();
        Button_HuntingAreaSelect.onClick.AddListener(OnClickHuntingAreaSelect);
    }

    private void OnClickBackToCharacterSelect()
    {
        _viewModel?.RequestBackToCharacterSelect();
    }

    private void OnClickHuntingAreaSelect()
    {
        _viewModel?.RequestHuntingAreaSelect();
    }
}
