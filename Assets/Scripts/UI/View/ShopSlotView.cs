using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotView : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private TMP_Text Text_Name;
    [SerializeField] private TMP_Text Text_Price;
    [SerializeField] private Button Button_Buy;
    [SerializeField] private TooltipTrigger TooltipTrigger;

    private ItemData _item;
    private Action<ItemData> _onBuyClicked;

    public void Setup(ItemData item, Action<ItemData> onBuyClicked)
    {
        _item = item;
        _onBuyClicked = onBuyClicked;

        Text_Name.text = item.Name;
        Text_Price.text = $"{item.Price}G";

        SpriteLoaderUtil.LoadAsync(Image_Icon, item.IconAddress).Forget();

        string usageHint = GetUsageHint(item.UsageType);
        string priceText = $"Buy: {item.Price}";

        TooltipData tooltipData = new TooltipData(
            item.IconAddress,
            item.Name,
            item.Description,
            usageHint,
            null,
            priceText,
            null
            );

        TooltipTrigger.SetData(tooltipData);

        Button_Buy.onClick.RemoveAllListeners();
        Button_Buy.onClick.AddListener(OnClickBuy);
    }

    private void OnClickBuy()
    {
        _onBuyClicked?.Invoke(_item);
    }

    private string GetUsageHint(ItemUsageType usageType)
    {
        switch (usageType)
        {
            case ItemUsageType.Consumable: return "Use to Mouse Right Click";
            case ItemUsageType.Equipment: return "Equip to Mouse Right Click";
            default: return null;
        }
    }
}
