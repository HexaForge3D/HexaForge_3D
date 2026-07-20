using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeViewModel
{
    private readonly string _slotId;
    
    public SkillTreeViewModel(string slotId)
    {
        _slotId = slotId;
    }

    public int GetAvailablePoints()
    {
        CharacterSaveData data = SaveManager.Instance.GetChararcterData(_slotId);

        if (data == null || data.Skills == null) return 0;

        return data.Skills.AvailablePoints;
    }

    public List<SkillTreeSlotData> GetSkillSlots()
    {
        List<SkillTreeSlotData> result = new List<SkillTreeSlotData>();

        CharacterSaveData saveData = SaveManager.Instance.GetChararcterData(_slotId);

        if (saveData == null) return result;

        int characterLevel = PlayerLevel.LevelFromExp(saveData.Exp);

        Dictionary<string, SkillTableData> table = GameDataManager.Instance.GetAllData<SkillTableData>();

        if (table == null) return result;

        foreach (SkillTableData skillTable in table.Values)
        {
            int currentLevel = SkillUtil.Instance.GetSkillLevel(skillTable.ID);
            bool isUnlocked = characterLevel >= skillTable.RequiredLevel;

            result.Add(new SkillTreeSlotData(skillTable, currentLevel, isUnlocked));
        }

        return result;
    }

    public void RequestUpgrade(string skillId)
    {
        SkillUtil.Instance.UpgradeSkill(skillId);
    }

    public void RequestDowngrade(string skillId)
    {
        SkillUtil.Instance.DowngradeSkill(skillId);
    }

    public void RequestResetAll()
    {
        SkillUtil.Instance.ResetAllSkills();
    }
}
