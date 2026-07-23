using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    public static event Action<string, DroppedItem> OnItemPickup;
    private List<DroppedItem> _nearbyItems = new List<DroppedItem>();
    private PlayerController _playerController;

    private int _targetIndex = 0;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        PlayerInputSystem.OnInteract += PickUpItem;
    }

    private void OnDisable()
    {
        PlayerInputSystem.OnInteract -= PickUpItem;
    }

    private void OnTriggerEnter(Collider other)
    {
        DroppedItem item = other.GetComponent<DroppedItem>();
        if (item != null && (_nearbyItems.Contains(item) == false))
        {
            _nearbyItems.Add(item);
            _targetIndex = 0;
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        DroppedItem item = other.GetComponent<DroppedItem>();
        if (item != null &&  _nearbyItems.Contains(item))
        {
            _nearbyItems.Remove(item);
            _targetIndex = 0;
        }
    }

    private void PickUpItem()
    {
        int beforeCount = _nearbyItems.Count;

        for (int i = _nearbyItems.Count - 1; i >= 0; i--)
        {
            if (_nearbyItems[i] == null)
            {
                _nearbyItems.RemoveAt(i);
            }
        }

        // 아이템을 주워서 떨어진 아이템 갯수가 줄어들었다면 인덱스를 0으로 리셋시킴
        if (_nearbyItems.Count < beforeCount)
        {
            _targetIndex = 0;
        }

        // 주울 아이템 없으면 0으로 다시 리셋한 후 종료
        if (_nearbyItems.Count == 0)
        {
            _targetIndex = 0;
            return;
        }

        if (_playerController == null || _playerController.PlayerData == null) return;
        
        // 인덱스가 남은 아이템 갯수를 넘어갈 경우, 다시 0으로 돌아오게 만듦 => 무한루프
        if (_targetIndex >= _nearbyItems.Count)
        {
            _targetIndex = 0;
        }
       
        DroppedItem targetItem = _nearbyItems[0];
        OnItemPickup?.Invoke(_playerController.PlayerData.SlotId, targetItem);

        _targetIndex++;
    }
}