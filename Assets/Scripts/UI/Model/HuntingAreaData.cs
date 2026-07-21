public class HuntingAreaData
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ImageAddress { get; private set; }
    public string MapName { get; private set; }
    public string MapType { get; private set; }
    public string PrefabPath { get; private set; }

    public HuntingAreaData(string id, string name, string description, string imageAddress, string mapName, string mapType, string prefabPath)
    {
        Id = id;
        Name = name;
        Description = description;
        ImageAddress = imageAddress;
        MapName = mapName;
        MapType = mapType;
        PrefabPath = prefabPath;
    }
}
