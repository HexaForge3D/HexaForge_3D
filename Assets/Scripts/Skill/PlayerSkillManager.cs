using System;
using System.Collections.Generic;
using UnityEngine;

// 스킬은 항상 프리팹화 시킬 것
public class PlayerSkillManager : MonoBehaviour
{
    [Serializable]
    public struct SkillVFXData
    {
        public string skillID; //Json과 이름을 똑같이 해야 함
        public GameObject vfxPrefab; // 콜라이더랑 SkillHitBOx 스크립트를 붙인 프리팹이 필요
    }

    [Header("Skill VFX Settings")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private List<SkillVFXData> _skillVFXList;

    private Dictionary<string, GameObject> _vfxDictionary = new Dictionary<string, GameObject>();

    private PlayerController _playerController;
    private PlayerBattle _playerBattle;

    // 현재 시전 중인 스킬의 데이터를 기억해둡니다. (애니메이션 이벤트 때 써먹기 위해)
    private SkillTableData _currentCastingSkill;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerBattle = GetComponent<PlayerBattle>();

        foreach (var data in _skillVFXList)
        {
            if (_vfxDictionary.ContainsKey(data.skillID) == false)
            {
                _vfxDictionary.Add(data.skillID, data.vfxPrefab);
            }
        }
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

        if (_vfxDictionary.TryGetValue(_currentCastingSkill.ID, out GameObject vfxPrefab))
        {
            if (vfxPrefab != null && _attackPoint != null)
            {
                // 1. 이펙트 생성
                GameObject spawnedVFX = Instantiate(vfxPrefab, _attackPoint.position, transform.rotation);

                // 2. 생성된 이펙트에 있는 SkillHitbox를 찾아서 데미지 발생
                SkillHitbox hitbox = spawnedVFX.GetComponent<SkillHitbox>();
                if (hitbox != null)
                {
                    hitbox.SetDamage(_currentCastingSkill.Damage);
                }
                else
                {
                    Debug.LogWarning("이펙트 프리팹에 SkillHitbox 스크립트가 없습니다!");
                }
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