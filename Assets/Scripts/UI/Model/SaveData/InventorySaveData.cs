using System;
using System.Collections.Generic;

[Serializable]
public class InventorySlotSaveData
{
    public string ItemId;
    public int Count;
}

[Serializable]
public class InventorySaveData
{
    public List<InventorySlotSaveData> Slots;
}