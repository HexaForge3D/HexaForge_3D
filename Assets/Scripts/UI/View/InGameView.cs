using UnityEngine;
using UnityEngine.UI;

public class InGameView : BaseUI
{

    private InGameViewModel _viewModel;

    public void BindViewModel(InGameViewModel viewModel)
    {
        _viewModel = viewModel;
    }
}
