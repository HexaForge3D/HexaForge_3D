using System.Collections.Generic;
using System.Linq;

public class VillageAreaRepository
{
    public List<VillageAreaData> GetAllAreas()
    {
        Dictionary<string, VillageAreaTableData> table = GameDataManager.Instance.GetAllData<VillageAreaTableData>();

        if (table == null)
        {
            return new List<VillageAreaData>();
        }

        return table.Values.Select(ConvertToVillageAreaData).ToList();
    }

    public VillageAreaData GetArea(string id)
    {
        VillageAreaTableData tableData = GameDataManager.Instance.GetData<VillageAreaTableData>(id);
        return tableData == null ? null : ConvertToVillageAreaData(tableData);
    }

    private VillageAreaData ConvertToVillageAreaData(VillageAreaTableData tableData)
    {
        return new VillageAreaData(tableData.ID, tableData.Name, tableData.Description, tableData.PrefabPath, tableData.MapName);
    }
}
