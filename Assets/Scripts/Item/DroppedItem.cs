using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemTableData ItemData { get; private set; }
    public int Amount { get; private set; }

    public void SetUp(ItemTableData itemData, int amount)
    {
        ItemData = itemData;
        Amount = amount;
        Transform targetGroup = null;

        if (RoomFieldManager.Instance != null)
        {
            targetGroup = RoomFieldManager.Instance._itemGroup.transform;
        }

        else if (DefenceFieldManager.Instance != null)
        {
            targetGroup = DefenceFieldManager.Instance._itemGroup.transform;
        }

        else if (NPCEscortFieldManager.Instance != null)
        {
            targetGroup = NPCEscortFieldManager.Instance._itemGroup.transform;
        }

        if (targetGroup != null)
        {
            transform.SetParent(targetGroup);
        }

        else
        {
            Debug.LogWarning("[DroppedItem] 현재 활성화된 던전 매니저의 _itemGroup을 찾을 수 없습니다!");
        }
    }

}