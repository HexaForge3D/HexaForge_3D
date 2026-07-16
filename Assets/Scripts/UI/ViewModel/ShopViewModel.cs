using System;
using System.Collections.Generic;

public class ShopViewModel
{
    private readonly ItemRepository _itemRepository = new ItemRepository();
    private readonly string _slotId;

    public Action OnGoldChanged;
    public Action<string> OnBuyFailed;

    public ShopViewModel(string slotId)
    {
        _slotId = slotId;
    }

    public List<ItemData> GetShopItems()
    {
        return _itemRepository.GetAllItems();
    }

    public int GetCurrentGold()
    {
        CharacterSaveData data = FindCurrentSlot();
        return data?.Gold ?? 0;
    }

    public void BuyItem(ItemData item)
    {
        bool success = SaveManager.Instance.BuyItem(_slotId, item.Id);

        if (success)
        {
            OnGoldChanged?.Invoke();
        }
        else
        {
            OnBuyFailed?.Invoke("Not Enough gold or max stack reached");
        }
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
