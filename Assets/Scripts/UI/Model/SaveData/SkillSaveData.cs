using System;
using System.Collections.Generic;

[Serializable]
public class SkillProgressData
{
    public string SkillId;
    public int SkillLevel = 1;
}

[Serializable]
public class SkillSaveData
{
    public List<SkillProgressData> Skills;
    public int AvailablePoints;
}
