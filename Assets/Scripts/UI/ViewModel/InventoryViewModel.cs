using System.Collections.Generic;

public class InventoryViewModel
{
    private readonly ItemRepository _itemRepository = new ItemRepository();

    public List<ItemData> GetInventorySlots(int slotCount)
    {
        List<ItemData> items = _itemRepository.GetAllItems();
        List<ItemData> result = new List<ItemData>();

        for (int i = 0; i < slotCount; i++)
        {
            if (i < items.Count)
            {
                result.Add(items[i]);
            }
            else
            {
                result.Add(null);
            }
        }

        return result;
    }
}
