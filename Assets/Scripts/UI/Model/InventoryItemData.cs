using UnityEngine;

public class InventoryItemData
{
    public string Id {  get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string IconAddress { get; private set; }
    public int MaxStack { get; private set; }
    public ItemUsageType UsageType { get; private set; }
    public int Price { get; private set; }
    public int Count { get; private set; }

    public InventoryItemData(ItemData item, int count)
    {
        Id = item.Id;
        Name = item.Name;
        Description = item.Description;
        IconAddress = item.IconAddress;
        MaxStack = item.MaxStack;
        UsageType = item.UsageType;
        Price = item.Price;
        Count = count;
    }
}
