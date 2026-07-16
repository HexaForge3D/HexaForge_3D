using UnityEngine;
using System.Collections.Generic;

public class MonsterHitBox : MonoBehaviour
{
    [Header("타격 설정")]
    public List<Transform> weaponPoints;
    public float hitRadius = 1.0f;

    [Header("데미지 설정")]
    public int minDamage = 10;
    public int maxDamage = 20;

    public void AttackHitCheck(int pointIndex)
    {
        if (weaponPoints == null || pointIndex < 0 || pointIndex >= weaponPoints.Count) return;

        Transform currentPoint = weaponPoints[pointIndex];
        if (currentPoint == null) return;

        Collider[] hitColliders = Physics.OverlapSphere(currentPoint.position, hitRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                int finalDamage = Random.Range(minDamage, maxDamage + 1);
                Debug.Log($"{currentPoint.name}으로 공격 성공! 데미지: {finalDamage}");

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
        if (weaponPoints == null) return;
        
        Gizmos.color = Color.red;
        foreach (var point in weaponPoints)
        {
            if (point != null) Gizmos.DrawWireSphere(point.position, hitRadius);
        }
        
    }
}
