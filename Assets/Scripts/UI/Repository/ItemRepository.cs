using System;
using System.Collections.Generic;

public class ItemRepository
{
    public List<ItemData> GetAllItems()
    {
        Dictionary<string, ItemTableData> table = GameDataManager.Instance.GetAllData<ItemTableData>();
        List<ItemData> result = new List<ItemData>();

        if (table == null) return result;

        foreach (ItemTableData tableData in table.Values)
        {
            result.Add(ConvertToItemData(tableData));
        }

        return result;
    }

    public ItemData GetItem(string id)
    {
        ItemTableData tableData = GameDataManager.Instance.GetData<ItemTableData>(id);
        return tableData == null ? null : ConvertToItemData(tableData);
    }

    private ItemData ConvertToItemData(ItemTableData tableData)
    {
        ItemUsageType usageType = ParseUsageType(tableData.UsageType);

        return new ItemData(
            tableData.ID,
            tableData.Name,
            tableData.Description,
            tableData.IconAddress,
            tableData.MaxStack,
            usageType,
            tableData.Price
        );
    }

    private ItemUsageType ParseUsageType(string usageTypeString)
    {
        if (Enum.TryParse(usageTypeString, out ItemUsageType result))
        {
            return result;
        }

        return ItemUsageType.None;
    }
}
