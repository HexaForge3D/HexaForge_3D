using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    public static event Action<string, DroppedItem> OnItemPickup;
    private List<DroppedItem> _nearbyItems = new List<DroppedItem>();
    private PlayerController _playerController;

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
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        DroppedItem item = other.GetComponent<DroppedItem>();
        if (item != null &&  _nearbyItems.Contains(item))
        {
            _nearbyItems.Remove(item);
        }
    }

    private void PickUpItem()
    {
        for (int i = _nearbyItems.Count - 1; i >= 0; i--)
        {
            if (_nearbyItems[i] == null)
            {
                _nearbyItems.RemoveAt(i);
            }
        }

        if (_nearbyItems.Count == 0) return;

        if (_playerController == null || _playerController.PlayerData == null) return;
        
        DroppedItem targetItem = _nearbyItems[0];

        OnItemPickup?.Invoke(_playerController.PlayerData.SlotId, targetItem);
    }
}