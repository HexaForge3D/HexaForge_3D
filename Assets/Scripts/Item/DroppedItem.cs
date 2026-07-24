using TMPro;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemTableData ItemData { get; private set; }
    public int Amount { get; private set; }

    [SerializeField] private TMP_Text Text_ItemName;
    [SerializeField] private Transform NameAnchor;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = GameObject.FindGameObjectWithTag("PlayerCamera")?.GetComponent<Camera>();
    }

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

        if (Text_ItemName != null && itemData != null)
        {
            Text_ItemName.text = itemData.Name;
        }
    }

    public void LateUpdate()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("PlayerCamera")?.GetComponent<Camera>();
        }

        if (NameAnchor != null && _mainCamera != null)
        {
            NameAnchor.forward = _mainCamera.transform.forward;
        }
    }

}