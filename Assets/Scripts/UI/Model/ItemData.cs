using UnityEngine;

public class ItemData
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string IconAddress { get; private set; }
    public int MaxStack { get; private set; }
    public ItemUsageType UsageType { get; private set; }
    public int Price { get; private set; }

    public ItemData(string id, string name, string description, string iconAddress, int maxStack, ItemUsageType usageType, int price)
    {
        Id = id;
        Name = name;
        Description = description;
        IconAddress = iconAddress;
        MaxStack = maxStack;
        UsageType = usageType;
        Price = price;
    }
}
