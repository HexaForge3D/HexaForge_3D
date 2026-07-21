using UnityEngine;

public class EquipmentManager : BaseMonoManager<EquipmentManager>
{
    public int GetRequiredLevel(EquipmentRank rank)
    {
        switch (rank)
        {
            case EquipmentRank.Normal: return 1;
            case EquipmentRank.Rare: return 10;
            case EquipmentRank.Epic: return 50;
            default: return 1; // 시간이 있을 시 등급과 레벨 더 추가할 것
        }
    }

    public TransactionResult EquipItem(string slotId, string itemId)
    {
        CharacterSaveData slot = SaveManager.Instance.GetChararcterData(slotId);
        if (slot == null) return TransactionResult.SlotNotFound;

        EquipmentTableData equipmentData = GameDataManager.Instance.GetData<EquipmentTableData>(itemId);
        if (equipmentData == null) return TransactionResult.ItemNotFound;

        int currentLevel = PlayerLevel.LevelFromExp(slot.Exp);
        int requiredLevel = GetRequiredLevel(equipmentData.Rank);

        if (currentLevel < requiredLevel)
        {
            return TransactionResult.LevelNotEnough;
        }

        if (slot.Equipped == null)
        {
            slot.Equipped = new EquippedItemsSaveData();
        }

        string previousItemId = GetEquippedItemId(slot.Equipped, equipmentData.EquipSlot);
        SetEquippedItemId(slot.Equipped, equipmentData.EquipSlot, itemId);

        SaveManager.Instance.RemoveItem(slotId, itemId, 1);
        
        if (string.IsNullOrEmpty(previousItemId) == false)
        {
            SaveManager.Instance.AddItem(slotId, previousItemId, 1);
        }

        SaveManager.Instance.SaveCurrentState();
        return TransactionResult.Success;
    }

    public TransactionResult UnEquipItem(string slotId, string equipSlotName)
    {
        CharacterSaveData slot = SaveManager.Instance.GetChararcterData(slotId);
        if (slot == null || slot.Equipped == null) return TransactionResult.SlotNotFound;

        string itemId = GetEquippedItemId(slot.Equipped, equipSlotName);
        if (string.IsNullOrEmpty(itemId)) return TransactionResult.ItemNotFound;

        SetEquippedItemId(slot.Equipped, equipSlotName, null);

        SaveManager.Instance.AddItem(slotId, itemId, 1);
        SaveManager.Instance.SaveCurrentState();

        return TransactionResult.Success;
    }

    public int GetFinalAtk(string slotId)
    {
        CharacterSaveData slot = SaveManager.Instance.GetChararcterData(slotId);
        if (slot == null) return 0;

        int bonus = GetEquipmentStatBonus(slot.Equipped, isAtk: true);
        return slot.Atk + bonus;
    }

    public int GetFinalDef(string slotId)
    {
        CharacterSaveData slot = SaveManager.Instance.GetChararcterData(slotId);
        if (slot == null) return 0;

        int bonus = GetEquipmentStatBonus(slot.Equipped, isAtk: false);
        return slot.Def + bonus;
    }

    private int GetEquipmentStatBonus(EquippedItemsSaveData equipped, bool isAtk)
    {
        if (equipped == null) return 0;

        int bonus = 0;
        bonus += GetItemStatBonus(equipped.WeaponItemId, isAtk);
        bonus += GetItemStatBonus(equipped.HelmetItemId, isAtk);
        bonus += GetItemStatBonus(equipped.ChestItemId, isAtk);
        bonus += GetItemStatBonus(equipped.PantsItemId, isAtk);
        bonus += GetItemStatBonus(equipped.BootsItemId, isAtk);
        bonus += GetItemStatBonus(equipped.GlovesItemId, isAtk);
        return bonus;
    }

    private int GetItemStatBonus(string itemId, bool isAtk)
    {
        if (string.IsNullOrEmpty(itemId)) return 0;
        EquipmentTableData equipmentData = GameDataManager.Instance.GetData<EquipmentTableData>(itemId);
        if (equipmentData == null) return 0;

        return isAtk ? equipmentData.AtkBonus : equipmentData.DefBonus;
    }

    public string GetEquippedItemId(string slotId, string equipSlot)
    {
        CharacterSaveData slot = SaveManager.Instance.GetChararcterData(slotId);
        if (slot == null || slot.Equipped == null) return null;

        return GetEquippedItemId(slot.Equipped, equipSlot);
    }

    private string GetEquippedItemId(EquippedItemsSaveData equipped, string equipSlot)
    {
        switch (equipSlot)
        {
            case "Weapon": return equipped.WeaponItemId;
            case "Helmet": return equipped.HelmetItemId;
            case "Chest": return equipped.ChestItemId;
            case "Pants": return equipped.PantsItemId;
            case "Boots": return equipped.BootsItemId;
            case "Gloves": return equipped.GlovesItemId;
            default: return null;
        }
    }

    private void SetEquippedItemId(EquippedItemsSaveData equipped, string equipSlot, string itemId)
    {
        switch (equipSlot)
        {
            case "Weapon": equipped.WeaponItemId = itemId; break;
            case "Helmet": equipped.HelmetItemId = itemId; break;
            case "Chest": equipped.ChestItemId = itemId; break;
            case "Pants": equipped.PantsItemId = itemId; break;
            case "Boots": equipped.BootsItemId = itemId; break;
            case "Gloves": equipped.GlovesItemId = itemId; break;
        }
    }

}