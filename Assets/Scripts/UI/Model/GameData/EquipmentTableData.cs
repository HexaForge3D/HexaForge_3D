using System;

public enum EquipSlotType
{
    Weapon,
    Helmet,
    Chest,
    Pants,
    Boots,
    Gloves,
}

[Serializable]
public class EquipmentTableData : GameDataBase
{
    public string EquipSlot;
    public int AtkBonus;
    public int DefBonus;
}