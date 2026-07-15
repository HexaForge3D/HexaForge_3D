using System.Collections.Generic;
using System.Linq;

public class HuntingAreaRepository
{
    public List<HuntingAreaData> GetAllAreas()
    {
        Dictionary<string, HuntingAreaTableData> table = GameDataManager.Instance.GetAllData<HuntingAreaTableData>();

        if (table == null)
        {
            return new List<HuntingAreaData>();
        }

        return table.Values.Select(ConvertToHuntingAreaData).ToList();
    }

    public HuntingAreaData GetArea(string id)
    {
        HuntingAreaTableData tableData = GameDataManager.Instance.GetData<HuntingAreaTableData>(id);
        return tableData == null ? null : ConvertToHuntingAreaData(tableData);
    }

    private HuntingAreaData ConvertToHuntingAreaData(HuntingAreaTableData tableData)
    {
        return new HuntingAreaData(tableData.ID, tableData.Name, tableData.Description, tableData.ImageAddress, tableData.MapName);
    }
}
