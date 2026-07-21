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

public enum EquipmentRank
{
    Normal = 0,
    Rare = 1,
    Epic = 2
}

[Serializable]
public class EquipmentTableData : GameDataBase
{
    public string EquipSlot;
    public int AtkBonus;
    public int DefBonus;
    public EquipmentRank Rank;
}