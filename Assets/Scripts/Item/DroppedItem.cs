using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemTableData ItemData { get; private set; }
    public int Amount { get; private set; }

    public void SetUp(ItemTableData itemData, int amount)
    {
        ItemData = itemData;
        Amount = amount;
    }

}