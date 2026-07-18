using System;

[Serializable]
public class CharacterSaveData
{
    public string SlotId;
    public bool IsEmpty = true;
    public string Name;
    public string Job;
    public int Exp;
    public int Hp;
    public int Mp;
    public int Atk;
    public int Def;
    public int Gold;

    public InventorySaveData Inventory;
    public SkillSaveData Skills;
    public EquippedItemsSaveData Equipped;
}

