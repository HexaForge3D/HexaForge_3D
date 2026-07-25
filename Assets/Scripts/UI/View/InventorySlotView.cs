using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private TMP_Text Text_Count;
    [SerializeField] private Image Image_CoolDownOverlay; 
    [SerializeField] private TooltipTrigger TooltipTrigger;

    private InventoryItemData _data;
    private Action<InventoryItemData, int> _onSellRequested;
    private Action<InventoryItemData> _onEquipRequested;
    private Action<InventoryItemData> _onUseRequested;

    private float _coolDownRemaining;
    private float _coolDownDuration;

    private void Update()
    {
        if (_coolDownRemaining <= 0f) return;

        _coolDownRemaining -= Time.deltaTime;
        Image_CoolDownOverlay.fillAmount = Mathf.Clamp01(_coolDownRemaining / _coolDownDuration);

        if (_coolDownRemaining <= 0f)
        {
            Image_CoolDownOverlay.gameObject.SetActive(false);
        }
    }

    public void Setup(InventoryItemData data, Action<InventoryItemData, int> onSellRequested, Action<InventoryItemData> onEquipRequested, Action<InventoryItemData> onUseRequested)
    {
        _data = data;
        _onSellRequested = onSellRequested;
        _onEquipRequested = onEquipRequested;
        _onUseRequested = onUseRequested;

        Image_CoolDownOverlay.gameObject.SetActive(false);

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
        string requirementText = null;

        if (data.UsageType == ItemUsageType.Equipment)
        {
            EquipmentTableData equipmentData = GameDataManager.Instance.GetData<EquipmentTableData>(data.Id);

            if (equipmentData != null)
            {
                int requiredLevel = EquipmentManager.Instance.GetRequiredLevel(equipmentData.Rank);
                requirementText = $"Requires Level {requiredLevel} ({equipmentData.Rank})";
            }
        }

        string countText = showCount ? $"x{data.Count}" : null;

        bool isShopOpen = UIManager.Instance.IsActiveUI(UIType.ShopUI);
        string priceText = null;

        if (isShopOpen)
        {
            int unitSellPrice = Mathf.FloorToInt(data.Price * SaveManager.SellPriceRatio);
            int totalSellPrice = unitSellPrice * data.Count;
            priceText = $"Sell: {unitSellPrice}G (Right Click) \nSell All: {totalSellPrice}G (Shift + Right Click)";
        }

        TooltipData tooltipData = new TooltipData(
            data.IconAddress,
            data.Name,
            data.Description,
            usageHint,
            countText,
            priceText,
            null,
            requirementText
        );

        TooltipTrigger.SetData(tooltipData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_data == null) return;
        if (eventData.button != PointerEventData.InputButton.Right) return;

        bool isShopOpen = UIManager.Instance.IsActiveUI(UIType.ShopUI);

        if (isShopOpen)
        {
            bool isShiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            int count = isShiftHeld ? _data.Count : 1;
            _onSellRequested?.Invoke(_data, count);
            return;
        }

        if (_coolDownRemaining > 0f)
        {
            return;
        }

        if (!string.IsNullOrEmpty(_data.UseSFXName))
        {
            SoundManager.Instance.PlaySFXSound(_data.UseSFXName);
        }

        if (_data.UsageType == ItemUsageType.Equipment)
        {
            _onEquipRequested?.Invoke(_data);
        }
        else if (_data.UsageType == ItemUsageType.Consumable)
        {
            _onUseRequested?.Invoke(_data);
        }
    }

    public void StartCoolDown(float duration)
    {
        if (duration <= 0f) return;

        _coolDownDuration = duration;
        _coolDownRemaining = duration;

        Image_CoolDownOverlay.gameObject.SetActive(true);
        Image_CoolDownOverlay.fillAmount = 1f;
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
