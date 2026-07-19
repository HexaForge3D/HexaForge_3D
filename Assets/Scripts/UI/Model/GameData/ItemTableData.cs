using System;

public enum ItemUsageType
{
    None,
    Consumable,
    Equipment,

}

[Serializable]
public class ItemTableData : GameDataBase
{
    public string Name;
    public string Description;
    public string IconAddress;
    public int MaxStack;
    public string UsageType;
    public int Price;
}
