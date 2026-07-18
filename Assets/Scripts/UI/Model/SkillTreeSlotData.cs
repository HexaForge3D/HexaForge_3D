
public class SkillTreeSlotData
{
    public string Id {  get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string IconAddress { get; private set; }
    public string Key { get; private set; }
    public int RequiredLevel { get; private set; }
    public int MaxLevel { get; private set; }
    public int CurrentLevel { get; private set; }
    public bool IsUnlocked { get; private set; }

    public SkillTreeSlotData(SkillTableData table, int currentLevel, bool isUnlocked)
    {
        Id = table.ID;
        Name = table.Name;
        Description = table.Description;
        IconAddress = table.IconAddress;
        Key = table.Key;
        RequiredLevel = table.RequiredLevel;
        MaxLevel = table.MaxLevel;
        CurrentLevel = currentLevel;
        IsUnlocked = isUnlocked;
    }
}
