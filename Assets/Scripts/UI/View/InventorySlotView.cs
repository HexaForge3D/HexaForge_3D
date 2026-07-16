using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotView : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private TMP_Text Text_Count;
    [SerializeField] private TooltipTrigger TooltipTrigger;

    public void Setup(InventoryItemData data)
    {
        if (data == null)
        {
            Image_Icon.gameObject.SetActive(false);
            Text_Count.gameObject.SetActive(false);
            TooltipTrigger.SetData(null);
            return;
        }

        Image_Icon.gameObject.SetActive(true);
        SpriteLoaderUtil.LoadAsync(Image_Icon, data.IconAddress).Forget();

        bool showCount = data.MaxStack > 1;
        Text_Count.gameObject.SetActive(showCount);

        if (showCount)
        {
            Text_Count.text = data.Count.ToString();
        }

        string usageHint = GetUsageHint(data.UsageType);
        string countText = showCount ? $"x{data.Count}" : null;

        bool isShopOpen = UIManager.Instance.IsActiveUI(UIType.ShopUI);
        string priceText = null;

        if (isShopOpen)
        {
            int sellPrice = Mathf.FloorToInt(data.Price * SaveManager.SellPriceRatio);
            priceText = $"Sell: {sellPrice}G";
        }

        TooltipData tooltipData = new TooltipData(
            data.IconAddress,
            data.Name,
            data.Description,
            usageHint,
            countText,
            priceText
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
