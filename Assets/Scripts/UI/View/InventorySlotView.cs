using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private TMP_Text Text_Count;
    [SerializeField] private Image Image_CoolDownOverlay;
    [SerializeField] private TooltipTrigger TooltipTrigger;

    private InventoryItemData _data;
    private Action<InventoryItemData, int> _onSellRequested;
    private Action<InventoryItemData> _onEquipRequested;
    private Action<InventoryItemData> _onUseRequested;
    private Action<int, int> _onSlotDropped;
    private Action<int> _onDiscardRequested;

    private int _slotIndex;  
    private static InventorySlotView _draggedSlot; 
    private GameObject _dragIcon; 

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

    public void Setup(InventoryItemData data, Action<InventoryItemData, int> onSellRequested, Action<InventoryItemData> onEquipRequested, Action<InventoryItemData> onUseRequested, Action<int, int> onSlotDropped, Action<int> onDiscardRequested, int slotIndex)
    {
        _data = data;
        _onSellRequested = onSellRequested;
        _onEquipRequested = onEquipRequested;
        _onUseRequested = onUseRequested;
        _onSlotDropped = onSlotDropped;
        _onDiscardRequested = onDiscardRequested;
        _slotIndex = slotIndex;

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

        if (_data.UsageType == ItemUsageType.Equipment)
        {
            _onEquipRequested?.Invoke(_data);
        }
        else if (_data.UsageType == ItemUsageType.Consumable)
        {
            _onUseRequested?.Invoke(_data);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_data == null) return;

        _draggedSlot = this;

        Color color = Image_Icon.color;
        color.a = 0.3f;
        Image_Icon.color = color;

        Transform dragParent = UIManager.Instance.DragLayerCanvas != null
            ? UIManager.Instance.DragLayerCanvas.transform
            : transform.root;

        _dragIcon = new GameObject("DragIcon");
        _dragIcon.transform.SetParent(dragParent);
        _dragIcon.transform.SetAsLastSibling();

        RectTransform dragRect = _dragIcon.AddComponent<RectTransform>();
        dragRect.sizeDelta = Image_Icon.rectTransform.rect.size;

        Image dragImage = _dragIcon.AddComponent<Image>();
        dragImage.sprite = Image_Icon.sprite;
        dragImage.raycastTarget = false;

        if (Text_Count.gameObject.activeSelf) 
        {
            TMP_Text dragText = Instantiate(Text_Count, _dragIcon.transform);  
            dragText.raycastTarget = false;

            RectTransform textRect = dragText.rectTransform;
            textRect.anchorMin = new Vector2(1f, 0f);
            textRect.anchorMax = new Vector2(1f, 0f);
            textRect.pivot = new Vector2(1f, 0f);
            textRect.anchoredPosition = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIcon != null)
        {
            _dragIcon.transform.position = eventData.position;
        }
        else
        {
            Debug.LogWarning("[InventorySlotView] OnDrag - _dragIcon이 null입니다!");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Color color = Image_Icon.color;
        color.a = 1f;
        Image_Icon.color = color;

        bool droppedOnValidTarget = eventData.pointerEnter != null && eventData.pointerEnter.GetComponentInParent<InventorySlotView>() != null;

        if (droppedOnValidTarget == false && _data != null)
        {
            _onDiscardRequested?.Invoke(_slotIndex);
        }

        if (_dragIcon != null)
        {
            Destroy(_dragIcon);
        }

        _draggedSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_draggedSlot == null || _draggedSlot == this)
        {
            return;
        }

        if (_draggedSlot._dragIcon != null) 
        {
            Destroy(_draggedSlot._dragIcon);
            _draggedSlot._dragIcon = null;
        }

        _onSlotDropped?.Invoke(_draggedSlot._slotIndex, _slotIndex);
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