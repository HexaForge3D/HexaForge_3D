
public class EquipmentSlotData
{
    public string EquipSlot { get; private set; }
    public ItemData Item { get; private set; }

    public EquipmentSlotData(string equipSlot, ItemData item)
    {
        EquipSlot = equipSlot;
        Item = item;
    }
}