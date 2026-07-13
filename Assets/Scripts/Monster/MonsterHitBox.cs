using UnityEngine;

public class MonsterHitBox : MonoBehaviour
{
    [Header("타격 설정")]
    public Transform weaponPoint;
    public float hitRadius = 1.0f;

    [Header("데미지 설정")]
    public int minDamage = 10;
    public int maxDamage = 20;

    public void AttackHitCheck()
    {
        if (weaponPoint == null) return;

        Collider[] hitColliders = Physics.OverlapSphere(weaponPoint.position, hitRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                int finalDamage = Random.Range(minDamage, maxDamage + 1);

                Debug.Log("공격 성공! 데미지: " + finalDamage);

                PlayerBattle playerBattle = hit.GetComponent<PlayerBattle>();

                if (playerBattle != null)
                {
                    playerBattle.TakeDamage(finalDamage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (weaponPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(weaponPoint.position, hitRadius);
        }
    }
}
