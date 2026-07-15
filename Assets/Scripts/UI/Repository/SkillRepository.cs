using System.Collections.Generic;
using UnityEngine;

public class SkillRepository : MonoBehaviour
{
    public List<SkillData> GetAllSkills()
    {
        Dictionary<string, SkillTableData> table = GameDataManager.Instance.GetAllData<SkillTableData>();
        List<SkillData> result = new List<SkillData>();

        if (table == null) return result;
        
        foreach (SkillTableData tableData in table.Values)
        {
           result.Add(ConvertToSkillData(tableData));
        }

        return result;  
    }

    public SkillData GetSkill(string id)
    {
        SkillTableData tableData = GameDataManager.Instance.GetData<SkillTableData>(id);
        return tableData == null ? null : ConvertToSkillData(tableData);
    }

    public SkillData GetSkillByKey(string key)
    {
        Dictionary<string, SkillTableData> table = GameDataManager.Instance.GetAllData<SkillTableData>();

        if (table == null) return null;

        foreach (SkillTableData tableData in table.Values)
        {
            if (tableData.Key == key)
            {
                return ConvertToSkillData(tableData);
            }
        }

        return null;
    }

    private SkillData ConvertToSkillData(SkillTableData tableData)
    {
        return new SkillData(
            tableData.ID,
            tableData.Name,
            tableData.Description,
            tableData.Key,
            tableData.RequiredLevel,
            tableData.IconAddress,
            tableData.CoolDown
            );
    }
}
