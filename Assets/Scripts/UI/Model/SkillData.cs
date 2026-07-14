public class SkillData
{
    public string Id {  get; private set; }
    public string Name { get; private set; }
    public string Key { get; private set; }
    public int RequiredLevel { get; private set; }
    public string IconAddress {  get; private set; }
    public float CoolDown { get; private set; }

    public SkillData(string id, string name, string key, int requiredLevel, string iconAddress, float coolDown)
    {
        Id = id;
        Name = name;
        Key = key;
        RequiredLevel = requiredLevel;
        IconAddress = iconAddress;
        CoolDown = coolDown;
    }
}
