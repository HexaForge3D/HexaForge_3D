using System.Collections.Generic;

public class InGameViewModel
{
    private readonly SkillRepository _skillRepository = new SkillRepository();

    private static readonly string[] SkillKeys = { "SkillQ", "SkillW", "SkillE", "SkillR", "SkillA", "SkillS", "SkillD", "SkillF" };

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
}
