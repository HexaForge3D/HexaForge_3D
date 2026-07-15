using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotView : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private TMP_Text Text_Count;
    [SerializeField] private TooltipTrigger TooltipTrigger;

    public void Setup(ItemData item, int count = 0)
    {
        if (item == null || count <= 0)
        {
            Image_Icon.gameObject.SetActive(false);
            Text_Count.gameObject.SetActive(false);
            TooltipTrigger.SetData(null);
            return;
        }

        Image_Icon.gameObject.SetActive(true);

        bool showCount = item.MaxStack > 1;
        Text_Count.gameObject.SetActive(showCount);

        if (showCount)
        {
            Text_Count.text = count.ToString();
        }

        string usageHint = GetUsageHint(item.UsageType);
        string countText = showCount ? $"x{count}" : null;

        TooltipData tooltipData = new TooltipData(
            item.IconAddress,
            item.Name,
            item.Description,
            usageHint,
            null
            );

        TooltipTrigger.SetData(tooltipData);
    }

    private string GetUsageHint(ItemUsageType usageType)
    {
        switch (usageType)
        {
            case ItemUsageType.Consumable: return "Use to Mouse Right Click";
            case ItemUsageType.Equipment: return "Equip to Mouse Right Click ";
            default: return null;
        }
    }
}
