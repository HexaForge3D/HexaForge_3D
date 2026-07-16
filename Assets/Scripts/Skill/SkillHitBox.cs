using UnityEngine;

public class SkillHitbox : MonoBehaviour
{
    private int _damage;
    private bool _isDamageSet = false;

    public void SetDamage(int damage)
    {
        _damage = damage;
        _isDamageSet = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDamageSet == false) return;

        // 부딪힌 대상의 태그가 "Monster"라면
        if (other.CompareTag("Monster"))
        {
            MonsterHealth monster = other.GetComponent<MonsterHealth>();
            if (monster != null)
            {
                // 몬스터에게 데미지를 입힘
                monster.TakeDamage(_damage);
            }
        }
    }
}