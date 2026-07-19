using System.Collections.Generic;
using UnityEngine;

public class EquipmentView : BaseOverLayUI
{
    [SerializeField] private EquipmentSlotView SlotView_Weapon;
    [SerializeField] private EquipmentSlotView SlotView_Helmet;
    [SerializeField] private EquipmentSlotView SlotView_Chest;
    [SerializeField] private EquipmentSlotView SlotView_Pants;
    [SerializeField] private EquipmentSlotView SlotView_Boots;
    [SerializeField] private EquipmentSlotView SlotView_Gloves;

    private EquipmentViewModel _viewModel;
    private Dictionary<string, EquipmentSlotView> _slotViews;

    public void BindViewModel(EquipmentViewModel viewModel)
    {
        _viewModel = viewModel;

        _slotViews = new Dictionary<string, EquipmentSlotView>
        {
            { "Weapon", SlotView_Weapon },
            { "Helmet", SlotView_Helmet },
            { "Chest", SlotView_Chest },
            { "Pants", SlotView_Pants },
            { "Boots", SlotView_Boots },
            { "Gloves", SlotView_Gloves },
        };

        Refresh();
    }

    public void Refresh()
    {
        List<EquipmentSlotData> slots = _viewModel.GetEquipmentSlots();

        foreach (EquipmentSlotData slotData in slots)
        {
            if (_slotViews.TryGetValue(slotData.EquipSlot, out EquipmentSlotView slotView))
            {
                slotView.Setup(slotData, OnUnequipRequested);
            }
        }
    }

    private void OnUnequipRequested(string equipSlot)
    {
        _viewModel.RequestUnequip(equipSlot);
        Refresh();
    }
}