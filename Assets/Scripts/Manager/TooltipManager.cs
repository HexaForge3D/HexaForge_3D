using UnityEngine;

public class TooltipManager : BaseMonoManager<TooltipManager>
{
    [SerializeField] private TooltipView TooltipViewInstance;

    public void Show(TooltipData data, Vector2 screenPosition)
    {
        TooltipViewInstance.Show(data, screenPosition);
    }

    public void UpdatePosition(Vector2 screenPosition)
    {
        TooltipViewInstance.UpdatePosition(screenPosition);
    }

    public void Hide()
    {
        TooltipViewInstance.Hide();
    }
}
