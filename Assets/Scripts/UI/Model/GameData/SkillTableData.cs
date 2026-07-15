using System;

[Serializable]
public class SkillTableData : GameDataBase
{
    public string Name;
    public string Description;
    public string Key;
    public int RequiredLevel;
    public string IconAddress;
    public float CoolDown;
    public int Damage;
    public int ManaCost;
}
