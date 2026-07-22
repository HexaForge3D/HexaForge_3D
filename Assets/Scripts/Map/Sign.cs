using System;
using UnityEngine;

public class Sign : MonoBehaviour
{
    private string _mapPrefabPath = string.Empty;
    private bool _playerInRange = false;

    public static Action OnShowMapRequest;

    private void OnEnable()
    {
        _mapPrefabPath = "";
        PlayerInputSystem.OnInteract += HandleInteraction;
    }

    private void OnDisable()
    {
        PlayerInputSystem.OnInteract -= HandleInteraction;
    }

    private void HandleInteraction()
    {
        if (_playerInRange)
        {
            OnShowMapRequest?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            Debug.Log("맵을 보려면 상호작용을 누르세요"); // [TODO] 추후 팝업이나 아이콘으로 띄울 예정
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
        }
    }
}
