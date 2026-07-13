using UnityEngine;
using UnityEngine.AI;

public class MonsterHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 150;
    private int currentHealth;
    private bool isDead = false;

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
        isDead = true;
        Debug.Log("몬스터 사망");

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        Collider coll = GetComponent<Collider>();
        if (coll != null) coll.enabled = false;

        Behaviour[] allScripts = GetComponents<Behaviour>();
        foreach (Behaviour script in allScripts)
        {
            if (script.GetType().Name == "BehaviorAgent")
            {
                script.enabled = false;
            }
        }

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        Destroy(gameObject, 4f);
    }
}
