using System;
using System.Collections.Generic;

public class InGameViewModel
{
    private readonly SkillRepository _skillRepository = new SkillRepository();

    private static readonly string[] SkillKeys = { "SkillQ", "SkillW", "SkillE", "SkillR", "SkillA", "SkillS", "SkillD", "SkillF" };

    private readonly string _slotId;

    public Action<float> OnHpRatioChanged;
    public Action<int, int> OnHpValueChanged;
    public Action<float> OnMpRatioChanged;
    public Action<int, int> OnMpValueChanged;

    public InGameViewModel(string slotId)
    {
        _slotId = slotId;
    }

    public List<SkillData> GetSkillSlots()
    {
        List<SkillData> result = new List<SkillData>();

        CharacterSaveData saveData = SaveManager.Instance.GetChararcterData(_slotId);
        int characterLevel = saveData != null ? PlayerState.LevelFromExp(saveData.Exp) : 0;

        foreach (string key in SkillKeys)
        {
            SkillData skill = _skillRepository.GetSkillByKey(key);

            if (skill != null && characterLevel < skill.RequiredLevel) skill = null;
            result.Add(skill);
        }

        return result;
    }

    public void GetInitialHpMp(out int currentHp, out int maxHp, out int currentMp,  out int maxMp)
    {
        CharacterSaveData saveData = SaveManager.Instance.GetChararcterData(_slotId);

        if (saveData == null)
        {
            currentHp = 0;
            maxHp = 0;
            currentMp = 0;
            maxMp = 0;
            return;
        }

        currentHp = saveData.CurrentHp;
        maxHp = saveData.Hp;
        currentMp = saveData.CurrentMp;
        maxMp = saveData.Mp;
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
        OnMpValueChanged?.Invoke(currentMp, maxMp);
    }
}
