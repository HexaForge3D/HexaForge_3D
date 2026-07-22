using System;
using UnityEngine;
using UnityEngine.AI;

public class MonsterHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 150;
    private int currentHealth;
    private bool isDead = false;

    [Header("Reward")]
    public int dropExp = 30;

    public static event Action<int> OnMonsterDied;
    public event Action<MonsterHealth> OnMonsterDieCount;

    private void OnEnable()
    {
        PlayerBattle.OnPlayerDead += StopAttackOnPlayerDeath;
    }

    private void OnDisable()
    {
        PlayerBattle.OnPlayerDead -= StopAttackOnPlayerDeath;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"[몬스터 피격] -{damageAmount} 데미지 (남은체력: {currentHealth} / {maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("몬스터 사망");

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        Collider coll = GetComponent<Collider>();
        if (coll != null) coll.enabled = false;

        Unity.Behavior.BehaviorGraphAgent bt = GetComponent<Unity.Behavior.BehaviorGraphAgent>();
        if (bt != null) bt.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Die");
        }

        OnMonsterDieCount?.Invoke(this);
        OnMonsterDied?.Invoke(dropExp);
        
        Destroy(gameObject, 3f);
    }

    private void StopAttackOnPlayerDeath()
    {
        if (isDead) return;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if ( agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.ResetTrigger("Attack");
            anim.Play("idle_");
        }

        Unity.Behavior.BehaviorGraphAgent bt = GetComponent<Unity.Behavior.BehaviorGraphAgent>() ;
        if (bt != null)
        {
            bt.enabled = false;
        }

        Debug.Log($"{gameObject.name} : 플레이어 사망감지, 공격을 중지합니다");

        Invoke("RebootBrain", 0.5f);
    }

    private void RebootBrain()
    {
        if (isDead) return;

        Unity.Behavior.BehaviorGraphAgent bt = GetComponent<Unity.Behavior.BehaviorGraphAgent>();
        if (bt != null)
        {
            bt.enabled = true;
        }

        Debug.Log($"{gameObject.name} : 뇌 재부팅 완료! 기억이 지워졌으므로 순찰로 복귀합니다.");
    }
}
