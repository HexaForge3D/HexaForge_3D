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
    public string SkillType; // 스킬 타입 추가 (버프, 힐, 데미지)
    public float Duration; // 버프인 경우에만 사용하는 지속시간 (다른 스킬들은 0으로 하시면 됩니다)
    public int BuffValue; // 힐이나 버프의 수치를 지정하는 변수

    public int MaxLevel = 5; // 스킬 최대 레벨
    public int DamagePerLevel; // 레벨 당 데미지 상승량
    public int BuffValuePerLevel; // 레벨 당 버프/힐 수치 상승량
    public float CooldownDecreasePerLevel; // 레벨 당 쿨타임 감소량
    public int ManaCostPerLevel; // 레벨 당 마나 소모 증가/감소량
}
