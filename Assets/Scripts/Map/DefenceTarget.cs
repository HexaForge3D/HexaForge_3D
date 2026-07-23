using System;
using UnityEngine;

public class DefenceTarget : MonoBehaviour
{
    [SerializeField] private int _maxHp = 500;
    private int _currentHp;
    private bool _isDestroyed = false;
    private bool _isPlayerInCollider = false;
    private bool _hasRequestedStart = false;

    public static event Action OnTargetDestroyed;
    public static event Action OnDefenceStartRequested;

    public static event Action<int, int> OnTargetHpChanged;

    private void OnEnable()
    {
        PlayerInputSystem.OnInteract += HandleInteraction;
    }

    private void OnDisable()
    {
        PlayerInputSystem.OnInteract -= HandleInteraction;
    }

    private void Start()
    {
        _currentHp = _maxHp;
        OnTargetHpChanged?.Invoke(_currentHp, _maxHp);
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
        if (!_isPlayerInCollider || _isDestroyed || _hasRequestedStart)
        {
            return;
        }

        _hasRequestedStart = true;
        Debug.Log("<color=green>[방어 목표 상호작용]</color> 디펜스 시작 요청을 보냅니다.");
        OnDefenceStartRequested?.Invoke();
    }

    public void TakeDamage(int damage)
    {
        if (_isDestroyed) return;

        _currentHp -= damage;
        _currentHp = Mathf.Max(0, _currentHp);

        Debug.Log($"<color=orange>[방어 목표 피격]</color> 남은 체력: {_currentHp} / {_maxHp}");

        OnTargetHpChanged?.Invoke(_currentHp, _maxHp);

        if (_currentHp <= 0)
        {
            DestroyTarget();
        }
    }

    private void DestroyTarget()
    {
        if (_isDestroyed) return;
        _isDestroyed = true;

        Debug.Log("<color=red>[방어 목표 파괴]</color> 디펜스 실패!");
        OnTargetDestroyed?.Invoke();

        Destroy(gameObject);
    }
}