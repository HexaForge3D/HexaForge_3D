public class VillageAreaData
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ImageAddress { get; private set; }
    public string MapName { get; private set; }

    public VillageAreaData(string id, string name, string description, string imageAddress, string mapName)
    {
        Id = id;
        Name = name;
        Description = description;
        ImageAddress = imageAddress;
        MapName = mapName;
    }
}
