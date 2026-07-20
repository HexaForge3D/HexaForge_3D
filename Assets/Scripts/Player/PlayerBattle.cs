using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using JetBrains.Annotations;

public class PlayerBattle : MonoBehaviour
{
    [Header("Attack Setting")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius = 1f;
    [SerializeField] private float _horizontalAngle = 120f;
    [SerializeField] private float _verticalAngle = 60f;
    [SerializeField] private float _attackOffset = 1f;
    [SerializeField] private int _gizmoGridSegments = 10;

    private PlayerController _playerController;
    private bool _isDead = false;
    // 몬스터에게 플레이어가 죽었다는 사실을 알려주기 위한 변수
    public bool IsDead => _isDead;
    // 플레이어 체력 변경시 나오는 이벤트 변수
    public static event Action<int, int> OnHpChanged;
    // 플레이어 사망시 나오는 이벤트 변수
    public static event Action OnPlayerDead;
    // 마나 ID와 마나 쿨타임 시간
    private Dictionary<string, float> _potionCooldowns = new Dictionary<string, float>();

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public void ExecuteAttack()
    {
        if (_playerController == null || _playerController.PlayerData == null || _isDead) return;

        int atk = _playerController.GetTotalAtk();

        if (_attackPoint != null)
        {
            Vector3 origin = _attackPoint.position + _attackPoint.forward * _attackOffset;
            Collider[] hitColliders = Physics.OverlapSphere(origin, _attackRadius);

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Monster"))
                {
                    Vector3 closestPoint = hitCollider.ClosestPoint(_attackPoint.position);

                    Vector3 localPos = _attackPoint.InverseTransformPoint(closestPoint);

                    float angleH = Mathf.Atan2(localPos.x, localPos.z) * Mathf.Rad2Deg;
                    float angleV = Mathf.Atan2(localPos.y, Mathf.Sqrt(localPos.x * localPos.x + localPos.z * localPos.z)) * Mathf.Rad2Deg;

                    if (Mathf.Abs(angleH) <= _horizontalAngle / 2f && Mathf.Abs(angleV) <= _verticalAngle / 2f)
                    {
                        MonsterHealth monster = hitCollider.GetComponent<MonsterHealth>();
                        if (monster != null)
                        {
                            monster.TakeDamage(atk);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("AttackPoint가 설정되지 않았습니다.");
        }
    }

    public void TakeDamage(int MonsterDamage)
    {
        if (_playerController == null || _playerController.PlayerData == null || _isDead) return;

        CharacterSaveData data = _playerController.PlayerData;

        int def = _playerController.PlayerData.Def;
        int finalDamage = MonsterDamage - def;

        if (finalDamage < 0) finalDamage = 0;

        data.CurrentHp -= finalDamage;
        data.CurrentHp = Mathf.Max(0, data.CurrentHp);

        Debug.Log($"<color=red>[플레이어 피격]</color> -{finalDamage} 데미지 (남은체력: {data.CurrentHp} / {data.Hp})");
        // 이벤트를 위한 내용
        OnHpChanged?.Invoke(data.CurrentHp, data.Hp);

        SaveManager.Instance.SaveCurrentState();

        if (data.CurrentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        Debug.Log("플레이어가 사망했습니다...");
        OnPlayerDead?.Invoke();

        if (_playerController != null)
        {
            _playerController.MoveStop();
            _playerController.FireAnimationTrigger("isDie");
            _playerController.enabled = false;
        }
        // 사망 시 몬스터가 플레이어를 공격하지 못하게 하기 위하여 Collider와 NavMeshAgent 설정 해제
        // 다시 게임을 할려고 하면, 초기화되는 부분은 추가해야함
        Collider playerCollider = GetComponent<Collider>();
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }
        
        NavMeshAgent playerAgent = GetComponent<NavMeshAgent>();
        if (playerAgent != null)
        {
            playerAgent.enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (_attackPoint == null) return;

        Vector3 origin = _attackPoint.position + _attackPoint.forward * _attackOffset;

        float halfH = _horizontalAngle / 2f;
        float halfV = _verticalAngle / 2f;

        // 1. 꼭짓점으로 향하는 4개의 주요 경계선 그리기 (노란색/주황색 느낌)
        Gizmos.color = new Color(1f, 0.6f, 0f, 1f);

        Vector3 topLeft = origin + (_attackPoint.rotation * Quaternion.Euler(-halfV, -halfH, 0) * Vector3.forward) * _attackRadius;
        Vector3 topRight = origin + (_attackPoint.rotation * Quaternion.Euler(-halfV, halfH, 0) * Vector3.forward) * _attackRadius;
        Vector3 bottomLeft = origin + (_attackPoint.rotation * Quaternion.Euler(halfV, -halfH, 0) * Vector3.forward) * _attackRadius;
        Vector3 bottomRight = origin + (_attackPoint.rotation * Quaternion.Euler(halfV, halfH, 0) * Vector3.forward) * _attackRadius;

        Gizmos.DrawLine(origin, topLeft);
        Gizmos.DrawLine(origin, topRight);
        Gizmos.DrawLine(origin, bottomLeft);
        Gizmos.DrawLine(origin, bottomRight);

        // 2. 구면을 따라 입체 그물망(Grid) 그리기 (파란색)
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.8f);

        // 가로 선 그리기 (상하 각도를 조금씩 변화시키며)
        for (int i = 0; i <= _gizmoGridSegments; i++)
        {
            float vAngle = Mathf.Lerp(-halfV, halfV, (float)i / _gizmoGridSegments);
            DrawSphericalArc(origin, _attackRadius, vAngle, true, -halfH, halfH, _gizmoGridSegments);
        }

        // 세로 선 그리기 (좌우 각도를 조금씩 변화시키며)
        for (int i = 0; i <= _gizmoGridSegments; i++)
        {
            float hAngle = Mathf.Lerp(-halfH, halfH, (float)i / _gizmoGridSegments);
            DrawSphericalArc(origin, _attackRadius, hAngle, false, -halfV, halfV, _gizmoGridSegments);
        }
    }

    private void DrawSphericalArc(Vector3 origin, float radius, float fixedAngle, bool isHorizontalLine, float startAngle, float endAngle, int segments)
    {
        Vector3 prevPoint = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float sweepAngle = Mathf.Lerp(startAngle, endAngle, (float)i / segments);
            Vector3 point;

            if (isHorizontalLine)
            {
                point = origin + (_attackPoint.rotation * Quaternion.Euler(fixedAngle, sweepAngle, 0) * Vector3.forward) * radius;
            }
            else
            {
                point = origin + (_attackPoint.rotation * Quaternion.Euler(sweepAngle, fixedAngle, 0) * Vector3.forward) * radius;
            }

            if (i > 0)
            {
                Gizmos.DrawLine(prevPoint, point);
            }
            prevPoint = point;
        }
    }

    public void UsePotion(string itemId)
    {
        if (_playerController == null || _playerController.PlayerData == null || _isDead) return;

        ConsumableTableData potionData = GameDataManager.Instance.GetData<ConsumableTableData>(itemId);

        if (_potionCooldowns.TryGetValue(itemId, out float coolTimeEnd))
        {
            if (Time.time < coolTimeEnd)
            {
                float remainTime = coolTimeEnd - Time.time;
                Debug.Log($"<color=pink>[체력 포션 쿨타임]</color> {remainTime}초 후에 다시 사용할 수 있습니다.");
                return;
            }
        }

        CharacterSaveData data = _playerController.PlayerData;

        bool isConsumed = SaveManager.Instance.RemoveItem(data.SlotId, itemId, 1);

        if (isConsumed == false)
        {
            Debug.LogWarning("인벤토리에 물약이 존재하지 않습니다");
            return;
        }

        if (potionData.HpBonus > 0)
        {
            int healAmount = Mathf.FloorToInt(data.Hp * potionData.HpBonus);
            data.CurrentHp += healAmount;

            data.CurrentHp = Mathf.Min(data.CurrentHp, data.Hp);

            Debug.Log($"<color=green>[체력 회복]</color> +{healAmount} 회복! (현재체력: {data.CurrentHp} / {data.Hp})");

            OnHpChanged?.Invoke(data.CurrentHp, data.Hp);
        }

        _potionCooldowns[itemId] = Time.time + potionData.CoolTime;
        SaveManager.Instance.SaveCurrentState();
    }
}