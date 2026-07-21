using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopViewModel
{
    private readonly ItemRepository _itemRepository = new ItemRepository();
    private readonly string _slotId;

    public Action OnGoldChanged;

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
        return data != null ? data.Gold : 0;
    }

    public void BuyItem(ItemData item)
    {
        TransactionResult result = SaveManager.Instance.BuyItem(_slotId, item.Id);

        if (result == TransactionResult.Success)
        {
            OnGoldChanged?.Invoke();
        }
        else
        {
            string message = SaveManager.GetTransactionMessage(result);
            SystemMessageManager.Instance.Show(message);
        }
    }

    private CharacterSaveData FindCurrentSlot()
    {
        foreach (CharacterSaveData slot in SaveManager.Instance.CurrentSaveData.Slots)
        {
            if (slot.SlotId == _slotId)
            {
                return slot;
            }
        }
        return null;
    }
}