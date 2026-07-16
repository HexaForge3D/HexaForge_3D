using System.Collections.Generic;

public class InventoryViewModel
{
    private readonly string _slotId;
    private readonly ItemRepository _itemRepository = new ItemRepository();

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
            foreach (InventorySlotSaveData invSlot in saveData.Inventory.Slots)
            {
                ItemData item = _itemRepository.GetItem(invSlot.ItemId);

                if (item != null)
                {
                    result.Add(new InventoryItemData(item, invSlot.Count));
                }
            }   
        }

        while (result.Count < slotCount)
        {
            result.Add(null);
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
}
