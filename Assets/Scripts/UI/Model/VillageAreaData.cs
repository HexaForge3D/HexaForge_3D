public class VillageAreaData
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string PrefabPath { get; private set; }
    public string MapName { get; private set; }
    public PortalType SpawnPortalType { get; private set; }

    public VillageAreaData(string id, string name, string description, string prefabPath, string mapName, PortalType spawnPortalType)
    {
        Id = id;
        Name = name;
        Description = description;
        PrefabPath = prefabPath;
        MapName = mapName;
        SpawnPortalType = spawnPortalType;
    }
}
