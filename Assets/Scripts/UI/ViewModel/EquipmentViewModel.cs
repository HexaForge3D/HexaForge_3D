using System.Collections.Generic;

public class EquipmentViewModel
{
    private readonly string _slotId;
    private readonly ItemRepository _itemRepository = new ItemRepository();

    private static readonly string[] EquipSlots = { "Weapon", "Helmet", "Chest", "Pants", "Boots", "Gloves" };

    public EquipmentViewModel(string slotId)
    {
        _slotId = slotId;
    }

    public List<EquipmentSlotData> GetEquipmentSlots()
    {
        List<EquipmentSlotData> result = new List<EquipmentSlotData>();

        foreach (string equipSlot in EquipSlots)
        {
            string itemId = SaveManager.Instance.GetEquippedItemId(_slotId, equipSlot);
            ItemData item = string.IsNullOrEmpty(itemId) ? null : _itemRepository.GetItem(itemId);

            result.Add(new EquipmentSlotData(equipSlot, item));
        }

        return result;
    }

    public void RequestUnequip(string equipSlot)
    {
        SaveManager.Instance.UnequipItem(_slotId, equipSlot);
    }
}