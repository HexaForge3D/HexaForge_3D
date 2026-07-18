using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private TooltipTrigger TooltipTrigger;

    private EquipmentSlotData _data;
    private Action<string> _onUnequipRequested;

    public void Setup(EquipmentSlotData data, Action<string> onUnequipRequested)
    {
        _data = data;
        _onUnequipRequested = onUnequipRequested;

        if (data.Item == null)
        {
            Image_Icon.gameObject.SetActive(false);
            TooltipTrigger.SetData(null);
            return;
        }

        Image_Icon.gameObject.SetActive(true);
        SpriteLoaderUtil.LoadAsync(Image_Icon, data.Item.IconAddress).Forget();

        TooltipData tooltipData = new TooltipData(
            data.Item.IconAddress,
            data.Item.Name,
            data.Item.Description,
            "Unequip to Mouse Right Click",
            null,
            null
        );

        TooltipTrigger.SetData(tooltipData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_data == null || _data.Item == null)
        {
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Right)
        {
            return;
        }

        _onUnequipRequested?.Invoke(_data.EquipSlot);
    }
}