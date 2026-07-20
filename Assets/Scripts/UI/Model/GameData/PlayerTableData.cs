using System;

[Serializable]
public class PlayerTableData : GameDataBase
{
    public string PrefabAddress;
    public int Hp;
    public int Mp;
    public int Atk;
    public int Def;
    public int HpPerLevel; // 레벨업 시 증가하는 체력
    public int MpPerLevel; // 레벨업 시 증가하는 마나
    public int AtkPerLevel; // 레벨업 시 증가하는 공격력
    public int DefPerLevel; // 레벨업 시 증가하는 방어력
}