using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryViewModel
{
    private readonly string _slotId;
    private readonly ItemRepository _itemRepository = new ItemRepository();

    public Action OnItemSold;

    public InventoryViewModel(string slotId)
    {
        _slotId = slotId;
    }

    public List<InventoryItemData> GetInventorySlots(int slotCount)
    {
        List<InventoryItemData> result = new List<InventoryItemData>();
        CharacterSaveData saveData = FindCurrentSlot();

        if (saveData != null && saveData.Inventory != null)
        {
            for (int i = 0; i < saveData.Inventory.Slots.Count; i++)
            {
                InventorySlotSaveData invSlot = saveData.Inventory.Slots[i];

                if (invSlot == null || string.IsNullOrEmpty(invSlot.ItemId))
                {
                    result.Add(null);
                    continue;
                }

                ItemData item = _itemRepository.GetItem(invSlot.ItemId);

                if (item != null)
                {
                    result.Add(new InventoryItemData(item, invSlot.Count, i));
                }
                else
                {
                    result.Add(null);
                }
            }
        }

        return result;
    }

    private CharacterSaveData FindCurrentSlot()
    {
        foreach (CharacterSaveData slot in SaveManager.Instance.CurrentSaveData.Slots)
        {
            if (slot.SlotId == _slotId) return slot;
        }

        return null;
    }

    public int GetCurrentGold()
    {
        CharacterSaveData data = FindCurrentSlot();
        return data != null ? data.Gold : 0;
    }
}
