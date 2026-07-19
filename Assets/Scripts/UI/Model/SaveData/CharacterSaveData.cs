using System;
using System.Collections.Generic;

[Serializable]
public class CharacterSaveData
{
    public string SlotId;
    public bool IsEmpty = true;
    public string Name;
    public string Job;
    public int Exp;
    public int Hp; // 최대 체력으로 사용될 부분
    public int Mp; // 최대 마나로 사용될 부분
    public int Atk;
    public int Def;
    public int Gold;
    public int CurrentHp; // 현재 체력으로 전투 중에서 감소시킬 부분
    public int CurrentMp; // 현재 마나로 전투 중에서 감소시킬 부분

    public InventorySaveData Inventory;
    public SkillSaveData Skills;
}

