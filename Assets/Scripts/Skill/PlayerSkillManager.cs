using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

// 스킬은 항상 프리팹화 시킬 것
public class PlayerSkillManager : MonoBehaviour
{
    // 스킬을 추가할 때마다 추가해줘야 함
    [Header("Skill1 Settings")]
    [SerializeField] private string id_Skill1; // Skill의 id는 꼭 맞춰야 함 Json파일의 ID와 같이
    [SerializeField] private GameObject prefab_Skill1Effect;
    [SerializeField] private Transform location_Skill1;

    [Header("Skill2 Settings")]
    [SerializeField] private string id_Skill2; // Skill의 id는 꼭 맞춰야 함 Json파일의 ID와 같이
    [SerializeField] private GameObject prefab_Skill2Effect;
    [SerializeField] private Transform location_Skill2;

    [Header("Skill3 Settings")]
    [SerializeField] private string id_Skill3; // Skill의 id는 꼭 맞춰야 함 Json파일의 ID와 같이
    [SerializeField] private GameObject prefab_Skill3Effect;
    [SerializeField] private Transform location_Skill3;

    [Header("Skill4 Settings")]
    [SerializeField] private string id_Skill4; // Skill의 id는 꼭 맞춰야 함 Json파일의 ID와 같이
    [SerializeField] private GameObject prefab_Skill4Effect;
    [SerializeField] private Transform location_Skill4;


    private PlayerController _playerController;
    private PlayerBattle _playerBattle;

    // 현재 시전 중인 스킬의 데이터를 기억해둡니다. (애니메이션 이벤트 때 써먹기 위해)
    private SkillTableData _currentCastingSkill;
    private GameObject _currentSpawnedSkill;

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

        // 애니메이션 이벤트를 기다리는 것이 아니라 사용하자 마자 멈추도록
        _playerController.SetAttackAnimPlaying(true);

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

        int currentLevel = SkillUtil.Instance.GetSkillLevel(_currentCastingSkill.ID);
        int calcDamage = SkillUtil.Instance.GetCalculatedDamage(_currentCastingSkill, currentLevel);
        int calcBuffValue = SkillUtil.Instance.GetCalculatedBuffValue(_currentCastingSkill, currentLevel);

        GameObject prefabSkill = null;
        Transform skillLocation = null;

        // 계속 추가하기 스킬은 else if로 추가해주자

        // Q 스킬
        if (_currentCastingSkill.ID == id_Skill1)
        {
            prefabSkill = prefab_Skill1Effect;
            skillLocation = location_Skill1;
        }

        // W 스킬
        else if (_currentCastingSkill.ID == id_Skill2)
        {
            prefabSkill = prefab_Skill2Effect;
            skillLocation = location_Skill2;
        }

        // E 스킬
        else if (_currentCastingSkill.ID == id_Skill3)
        {
            prefabSkill = prefab_Skill3Effect;
            skillLocation = location_Skill3;
        }

        // R 스킬
        else if (_currentCastingSkill.ID == id_Skill4)
        {
            prefabSkill = prefab_Skill4Effect;
            skillLocation = location_Skill4;
        }

        if (prefabSkill != null && skillLocation != null)
        {
            GameObject spawnedSkill;

            if (_currentCastingSkill.SkillType == "Attack")
            {
                spawnedSkill = Instantiate(prefabSkill, skillLocation.position, transform.rotation);
            }
            else
            {
                spawnedSkill = Instantiate(prefabSkill, skillLocation.position, transform.rotation, transform);
            }

            _currentSpawnedSkill = spawnedSkill;

            SkillHitbox hitbox = spawnedSkill.GetComponent<SkillHitbox>();

            if (hitbox != null)
            {
                //레벨이 반영된   데미지를 스킬 히트박스에 설정
                hitbox.SetDamage(calcDamage);
            }

            else if (_currentCastingSkill.SkillType == "Attack")
            {
                Debug.LogWarning("Skill프리팹에 SkillHitBox가 있는지 확인하세요");
            }
        }

        switch (_currentCastingSkill.SkillType)
        {
            case "Attack":
                break;

            case "Heal":
                ApplyHeal(calcBuffValue);
                break;

            case "Buff":
                ApplyBuff(_currentCastingSkill, calcBuffValue, this.GetCancellationTokenOnDestroy()).Forget();
                break;

            default:
                Debug.LogWarning($"[PlayerSkillManager] 알 수 없는 스킬 타입: {_currentCastingSkill.SkillType}");
                break;
        }
    }

    public void SkillAnimEnd()
    {
        if (_currentSpawnedSkill != null)
        {
            Destroy(_currentSpawnedSkill);
            _currentSpawnedSkill = null;
        }

        // 스킬 사용 후 다시 움직이고 평타나 다른 스킬을 쓸 수 있게 수정
        _playerController.SetAttackAnimPlaying(false);
        _currentCastingSkill = null;
    }

    private void ApplyHeal(int calculatedHealAmount)
    {
        CharacterSaveData playerData = _playerController.PlayerData;
        if (playerData == null) return;

        playerData.CurrentHp += calculatedHealAmount;

        if (playerData.CurrentHp > playerData.Hp)
        {
            playerData.CurrentHp = playerData.Hp;
        }

        Debug.Log($"<color=green>[Heal] 체력 {calculatedHealAmount} 회복! 현재 HP: {playerData.Hp}</color>");
        SaveManager.Instance.SaveCurrentState();
    }

    private async UniTaskVoid ApplyBuff(SkillTableData buffData, int calculatedBuffValue, CancellationToken cancellationToken)
    {
        CharacterSaveData playerData = _playerController.PlayerData;

        if (playerData == null) return;

        bool isAtkBuff = buffData.ID.Contains("Rage") || buffData.ID.Contains("Atk");

        if (isAtkBuff)
        {
            _playerController.BuffAtk += calculatedBuffValue; // 계산된 버프값 적용
            Debug.Log($"<color=yellow>[Buff] {buffData.Name} 발동! 공격력 {calculatedBuffValue} 증가 ({buffData.Duration}초)</color>");
        }

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(buffData.Duration), cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        // 버프 효과 원상복구
        if (isAtkBuff)
        {
            _playerController.BuffAtk -= calculatedBuffValue;
            Debug.Log($"<color=orange>[Buff] {buffData.Name} 종료! 공격력 {calculatedBuffValue} 감소 원상복구</color>");
        }
    }

    public void CancelCurrentSkill()
    {
        if (_currentSpawnedSkill != null)
        {
            Destroy(_currentSpawnedSkill);
            _currentSpawnedSkill = null;
        }

        _currentCastingSkill = null;

        _playerController.SetAttackAnimPlaying(false);

        Debug.Log("<color=red>[스킬 캔슬]</color> 시전 중이던 스킬을 취소하고 회피합니다!");
    }
}