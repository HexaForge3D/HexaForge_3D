using System;
using System.Collections.Generic;
using UnityEngine;

// 스킬은 항상 프리팹화 시킬 것
public class PlayerSkillManager : MonoBehaviour
{
    // 스킬을 추가할 때마다 추가해줘야 함
    [Header("Skill1 Settings")]
    [SerializeField] private string id_Skill1; // Skill의 id는 꼭 맞춰야 함 Json파일의 ID와 같이
    [SerializeField] private GameObject prefab_Skill1Effect;
    [SerializeField] private Transform location_Skill1;

    [Header("Skill3 Settings")]
    [SerializeField] private string id_Skill3; // Skill의 id는 꼭 맞춰야 함 Json파일의 ID와 같이
    [SerializeField] private GameObject prefab_Skill3Effect;
    [SerializeField] private Transform location_Skill3;


    private PlayerController _playerController;
    private PlayerBattle _playerBattle;

    // 현재 시전 중인 스킬의 데이터를 기억해둡니다. (애니메이션 이벤트 때 써먹기 위해)
    private SkillTableData _currentCastingSkill;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerBattle = GetComponent<PlayerBattle>();
    }

    private void OnEnable()
    {
        SkillUtil.OnSkillSuccess += HandleSkillSuccess;
    }

    private void OnDisable()
    {
        SkillUtil.OnSkillSuccess -= HandleSkillSuccess;
    }

    private void HandleSkillSuccess(SkillTableData skillData)
    {
        _currentCastingSkill = skillData;

        // 마우스 방향으로 캐릭터 돌리기 및 이동 정지
        _playerController.LookAtMousePosition();

        // Animator의 Trigger 이름을 Skill.jSON의 ID와 동일하게 맞추기
        _playerController.FireAnimationTrigger(skillData.ID);
    }

    public void SkillAnimStart()
    {
        _playerController.SetAttackAnimPlaying(true);
    }

    public void SkillAttack()
    {
        if (_currentCastingSkill == null) return;

        GameObject prefabSkill = null;
        Transform skillLocation = null;
        // 계속 추가하기 스킬은 else if로 추가해주자: Q 스킬
        if (_currentCastingSkill.ID == id_Skill1)
        {
            prefabSkill = prefab_Skill1Effect;
            skillLocation = location_Skill1;
        }

        // E 스킬
        else if (_currentCastingSkill.ID == id_Skill3)
        {
            prefabSkill = prefab_Skill3Effect;
            skillLocation = location_Skill3;
        }


        if (prefabSkill != null && skillLocation != null)
        {
            GameObject spawnedSkill = Instantiate(prefabSkill, skillLocation.position, transform.rotation);

            SkillHitbox hitbox = spawnedSkill.GetComponent<SkillHitbox>();

            if (hitbox != null)
            {
                hitbox.SetDamage(_currentCastingSkill.Damage);
            }

            else
            {
                Debug.LogWarning("Skill프리팹에 SkillHitBox가 있는지 확인하세요");
            }
        }
    }

    public void SkillAnimEnd()
    {
        // 스킬 사용 후 다시 움직이고 평타나 다른 스킬을 쓸 수 있게 수정
        _playerController.SetAttackAnimPlaying(false);
        _currentCastingSkill = null;
    }
}