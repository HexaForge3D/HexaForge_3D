using System;
using System.Collections.Generic;
using UnityEngine;

public class InGameViewModel
{
    private readonly SkillRepository _skillRepository = new SkillRepository();

    private static readonly string[] SkillKeys = { "SkillQ", "SkillW", "SkillE", "SkillR", "SkillA", "SkillS", "SkillD", "SkillF" };

    private readonly string _slotId;

    public Action<float> OnHpRatioChanged;
    public Action<int, int> OnHpValueChanged;

    public Action<float> OnMpRatioChanged;
    public Action<int, int> OnMpValueChanged;

    public Action<float> OnExpRatioChanged;
    public Action<int, int> OnExpValueChanged;

    public InGameViewModel(string slotId)
    {
        _slotId = slotId;
    }

    public List<SkillData> GetSkillSlots()
    {
        List<SkillData> result = new List<SkillData>();

        CharacterSaveData saveData = SaveManager.Instance.GetChararcterData(_slotId);
        int characterLevel = saveData != null ? PlayerLevel.LevelFromExp(saveData.Exp) : 0;

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

    public void HandleExpChanged(int currentExp)
    {
        float ratio = GetExpRatio(currentExp);
        OnExpRatioChanged?.Invoke(ratio);

        int levelRangeCurrent;
        int levelRangeMax;

        GetExpRange(currentExp, out levelRangeCurrent, out levelRangeMax);
        OnExpValueChanged?.Invoke(levelRangeCurrent, levelRangeMax);
    }

    public float GetExpRatio(int currentExp)
    {
        PlayerLevel playerLevel = PlayerSpawnManager.Instance.GetPlayerLevel();

        if (playerLevel == null) return 0f;

        int currentLevel = PlayerLevel.LevelFromExp(currentExp);
        int levelStartExp = playerLevel.ExpForNextLevel(currentLevel);
        int nextLevelExp = playerLevel.ExpForNextLevel(currentLevel + 1);

        int range = nextLevelExp - levelStartExp;

        if (range <= 0) return 1f;

        return Mathf.Clamp01((float)(currentExp - levelStartExp) / range);
    }

    public void GetExpRange(int currentExp, out int current, out int max)
    {
        PlayerLevel playerLevel = PlayerSpawnManager.Instance.GetPlayerLevel();

        if (playerLevel == null)
        {
            current = 0;
            max = 0;
            return;
        }

        int currentLevel = PlayerLevel.LevelFromExp(currentExp);
        int levelStartExp = playerLevel.ExpForNextLevel(currentLevel);
        int nextLevelExp = playerLevel.ExpForNextLevel(currentLevel + 1);

        current = currentExp - levelStartExp;
        max = nextLevelExp - levelStartExp;
    }

    public int GetInitialExp()
    {
        CharacterSaveData saveData = SaveManager.Instance.GetChararcterData(_slotId);
        return saveData != null ? saveData.Exp : 0;
    }
}
