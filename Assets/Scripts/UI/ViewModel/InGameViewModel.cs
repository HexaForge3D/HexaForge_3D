using System;
using System.Collections.Generic;

public class InGameViewModel
{
    private readonly SkillRepository _skillRepository = new SkillRepository();

    private static readonly string[] SkillKeys = { "SkillQ", "SkillW", "SkillE", "SkillR", "SkillA", "SkillS", "SkillD", "SkillF" };

    public Action<float> OnHpRatioChanged;
    public Action<int, int> OnHpValueChanged;
    public Action<float> OnMpRatioChanged;

    public List<SkillData> GetSkillSlots()
    {
        List<SkillData> result = new List<SkillData>();

        foreach (string key in SkillKeys)
        {
            SkillData skill = _skillRepository.GetSkillByKey(key);
            result.Add(skill);
        }

        return result;
    }

    public void HandleHpChanged(int currentHp, int maxHp)
    {
        float ratio = maxHp > 0 ? (float)currentHp / maxHp : 0f;
        OnHpRatioChanged?.Invoke(ratio);
        OnHpValueChanged?.Invoke(currentHp, maxHp);
    }

    public void HandleMpChanged(int currentMp, int maxMp)
    {
        float ratio = maxMp > 0 ? (float)currentMp / maxMp : 0f;    
        OnMpRatioChanged?.Invoke(ratio);
    }
}
