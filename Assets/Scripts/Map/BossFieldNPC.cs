using System;
using UnityEngine;

public class BossFieldNPC : MonoBehaviour
{
    [SerializeField] private float _detectRadius = 3f;

    private bool _isPlayerInCollider = false;
    private bool _isRequested = false;

    public static event Action OnBossFieldStartRequested;

    private void OnEnable()
    {
        PlayerInputSystem.OnInteract += HandleInteraction;
    }

    private void OnDisable()
    {
        PlayerInputSystem.OnInteract -= HandleInteraction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInCollider = false;
        }
    }

    private void HandleInteraction()
    {
        if (!_isPlayerInCollider || _isRequested)
        {
            return;
        }

        _isRequested = true;
        Debug.Log("<color=green>[BossFieldNPC] 플레이어 상호작용! 보스전 시작 요청.</color>");
        OnBossFieldStartRequested?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectRadius);
    }
}