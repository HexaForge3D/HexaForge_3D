using UnityEngine;

public class PlayerBattle : MonoBehaviour
{
    [Header("Attack Setting")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius = 1f;

    private PlayerController _playerController;
    private int _currentHp;
    private bool _isDead = false;
    private bool _isHpSet = false;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_isHpSet == false && _playerController != null && _playerController.PlayerData != null)
        {
            _currentHp = _playerController.PlayerData.Hp;
            _isHpSet = true;
        }
    }

    public void ExecuteAttack()
    {
        if (_playerController == null || _playerController.PlayerData == null || _isDead) return;

        int atk = _playerController.PlayerData.Atk;
        Debug.Log($"플레이어가 공격을 합니다! (공격력: {atk})");

        if (_attackPoint != null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(_attackPoint.position, _attackRadius);

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Monster"))
                {
                    MonsterHealth monster = hitCollider.GetComponent<MonsterHealth>();
                    if (monster != null)
                    {
                        monster.TakeDamage(atk);
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

        int def = _playerController.PlayerData.Def;

        int finalDamage = MonsterDamage - def;

        if (finalDamage < 0) finalDamage = 0;

        _currentHp -= finalDamage;
        _currentHp = Mathf.Max(0, _currentHp);

        int maxHp = _playerController.PlayerData.Hp;

        Debug.Log($"<color=red>[플레이어 피격]</color> -{finalDamage} 데미지 (남은체력: {_currentHp} / {maxHp})");

        if (_currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        Debug.Log("플레이어가 사망했습니다...");

        if (_playerController != null)
        {
            _playerController.MoveStop();
            _playerController.enabled = false;
        }

    }

    private void OnDrawGizmos()
    {
        if (_attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
        }
    }
}