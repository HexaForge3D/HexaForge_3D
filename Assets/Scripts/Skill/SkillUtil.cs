using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillUtil : MonoBehaviour
{
    //외부에서 접근할 수 있도록 싱글톤 패턴으로 구현
    public static SkillUtil Instance { get; private set; }
    // 앞으로 스킬 관련된 공통 부분들은 이 스크립트에서 관리합니다. (07/14: 나영준, 함동현)
    // 초안으로 만들었기 떄문에 더 좋은 아이디어 있다면 수정 부탁드립니다. (07/15: 함동현)

    private PlayerController _playerController;
    // 쿨타임 개념 추가 => Key: 스킬ID(String), value: 스킬 쿨타임 (float)
    private Dictionary<string, float> _skillCoolTime = new Dictionary<string, float>();
    // 스킬 사용을 성공하였을 때 이벤트 발생
    public static event Action<SkillTableData> OnSkillSuccess;
    // 스킬 쿨타임 시작할 때 이벤트 발생
    public static event Action<string, float> OnSkillCoolTimeStart;
    // 스킬 쿨타임 중이라 스킬 사용에 실패했을 때
    public static event Action<string, float> OnSkillCoolTimeFail;
    // 마나가 부족할 때
    public static event Action<string> OnLackMana;
    //UI에서 스킬 데이터가 변경되었을 때 UI를 갱신하기 위해서 이벤트 발생
    public event Action OnSkillDataUpdated;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        // CharacterSaveData에서 Mp를 가져오기 위해서
        _playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        // PlayerInputSystem의 이벤트 구독하기 (나머지는 추가로 하면 됩니다 그때마다)
        PlayerInputSystem.OnSkill1 += HandleSkill1;
        PlayerInputSystem.OnSkill2 += HandleSkill2;
        PlayerInputSystem.OnSkill3 += HandleSkill3;
        PlayerInputSystem.OnSkill4 += HandleSkill4;
    }

    private void OnDisable()
    {
        // 스킬을 쓰고 나면 구독 해제하기 위해서
        PlayerInputSystem.OnSkill1 -= HandleSkill1;
        PlayerInputSystem.OnSkill2 -= HandleSkill2;
        PlayerInputSystem.OnSkill3 -= HandleSkill3;
        PlayerInputSystem.OnSkill4 -= HandleSkill4;
    }

    private void HandleSkill1() => UseSkill("Skill_Slash");
    private void HandleSkill2() => UseSkill("Skill_Rage");
    private void HandleSkill3() => UseSkill("Skill_Smash");
    private void HandleSkill4() => UseSkill("Skill_Bladestorm");

    // UI에서 스킬 데이터가 변경되었을 때 UI를 갱신하기 위해서 이벤트 발생
    public void InvokeSkillDataUpdated() => OnSkillDataUpdated?.Invoke();

    private void EnsureSkillDataExists()
    {
        if (_playerController.PlayerData.Skills == null)
            _playerController.PlayerData.Skills = new SkillSaveData();
        if (_playerController.PlayerData.Skills.Skills == null)
            _playerController.PlayerData.Skills.Skills = new List<SkillProgressData>();
    }

    
    public int GetSkillLevel(string skillId)
    {
        if (_playerController?.PlayerData == null) return 1;
        EnsureSkillDataExists();
        // 스킬 데이터가 존재하지 않으면 초기화
        List<SkillProgressData> skillList = _playerController.PlayerData.Skills.Skills;
        // 스킬 데이터에서 해당 스킬을 찾기
        SkillProgressData skillProg = skillList.Find(s => s.SkillId == skillId);

        if (skillProg == null)
        {
            // 스킬 데이터가 존재하지 않으면 초기화
            skillProg = new SkillProgressData { SkillId = skillId, SkillLevel = 1 };
            skillList.Add(skillProg);
        }

        // 스킬 레벨 반환
        return skillProg.SkillLevel;
    }
    // 스킬 레벨에 따라 데미지, 버프값, 마나 소모량, 쿨타임 등을 계산하는 메서드
    public int GetCalculatedDamage(SkillTableData data, int level) => data.Damage + (data.DamagePerLevel * (level - 1));
    public int GetCalculatedBuffValue(SkillTableData data, int level) => data.BuffValue + (data.BuffValuePerLevel * (level - 1));

    public int GetCalculatedManaCost(SkillTableData data, int level) => data.ManaCost + (data.ManaCostPerLevel * (level - 1));
    public float GetCalculatedCoolDown(SkillTableData data, int level)
    {
        float calcCoolDown = data.CoolDown - (data.CooldownDecreasePerLevel * (level - 1));
        return Mathf.Max(0.5f, calcCoolDown);
    }
    // 스킬 레벨 업그레이드 메서드
    public void UpgradeSkill(string skillId)
    {
        EnsureSkillDataExists();
        SkillSaveData skillSaveData = _playerController.PlayerData.Skills;
        SkillTableData skillTableData = GameDataManager.Instance.GetData<SkillTableData>(skillId);

        if (skillSaveData.AvailablePoints <= 0) return;

        SkillProgressData skillProg = skillSaveData.Skills.Find(s => s.SkillId == skillId);

        if (skillProg == null)
        {
            skillProg = new SkillProgressData { SkillId = skillId, SkillLevel = 1 };
            skillSaveData.Skills.Add(skillProg);
        }

        if (skillProg.SkillLevel >= skillTableData.MaxLevel) return;
        
        skillProg.SkillLevel++;
        skillSaveData.AvailablePoints--;
        SaveManager.Instance.SaveCurrentState();
        OnSkillDataUpdated?.Invoke();
    }
    // 스킬 레벨 다운그레이드 메서드
    public void DowngradeSkill(string skillId)
    {
        EnsureSkillDataExists();
        SkillSaveData skillSaveData = _playerController.PlayerData.Skills;

        SkillProgressData skillProg = skillSaveData.Skills.Find(s => s.SkillId == skillId);

        if (skillProg != null && skillProg.SkillLevel > 1)
        {
            skillProg.SkillLevel--;
            skillSaveData.AvailablePoints++;

            OnSkillDataUpdated?.Invoke();
        }
    }
    // 모든 스킬을 초기화하고 사용한 포인트를 반환하는 메서드
    public void ResetAllSkills()
    {
        EnsureSkillDataExists();
        SkillSaveData skillSaveData = _playerController.PlayerData.Skills;
        int refundedPoints = 0;

        foreach (SkillProgressData skillProg in skillSaveData.Skills)
        {
            if (skillProg.SkillLevel > 1)
            {
                refundedPoints += (skillProg.SkillLevel - 1);
                skillProg.SkillLevel = 1;
            }
        }

        skillSaveData.AvailablePoints += refundedPoints;
        OnSkillDataUpdated?.Invoke();
    }
    private void UseSkill(string skillId)
    {
        if (string.IsNullOrEmpty(skillId)) return;

        // 스킬 사용 중에는 플레이어가 다른 행동을 할 수 없게 하는 메서드
        if (_playerController.IsAttackingAnimPlaying) return;


        SkillTableData skillData = GameDataManager.Instance.GetData<SkillTableData>(skillId);

        if (skillData == null)
        {
            Debug.LogWarning("스킬 데이터를 찾을 수 없습니다.");
            return;
        }

        CharacterSaveData playerData = _playerController.PlayerData;
        if (playerData == null) return;

        if (_skillCoolTime.TryGetValue(skillData.ID, out float coolTime))
        {
            if (Time.time < coolTime)
            {
                float remainTime = coolTime - Time.time;
                Debug.Log($"쿨타임 입니다. 남은 시간: {remainTime}초");

                OnSkillCoolTimeFail?.Invoke(skillData.ID, remainTime);
                return;
            }
        }

        //현재 스킬 레벨 가져오기
        int currentLevel = GetSkillLevel(skillData.ID);
        int calcManaCost = GetCalculatedManaCost(skillData, currentLevel);
        float calcCooldown = GetCalculatedCoolDown(skillData, currentLevel);
        int calcDamage = GetCalculatedDamage(skillData, currentLevel);

        //계산된 마나 소모량으로 차감
        if (playerData.CurrentMp < calcManaCost)
        {
            Debug.Log("마나가 부족합니다!!");
            OnLackMana?.Invoke(skillData.ID);
            return;
        }
        playerData.CurrentMp -= calcManaCost;

        // 계산된 쿨타임 적용
        _skillCoolTime[skillData.ID] = Time.time + calcCooldown;

        OnSkillSuccess?.Invoke(skillData);
        OnSkillCoolTimeStart?.Invoke(skillData.ID, calcCooldown);

        Debug.Log($"<color=cyan>[SkillUtil] {skillData.Name} 발동! (Lv.{currentLevel})</color> 데미지: {calcDamage}, 남은 마나: {playerData.Mp}");
    }
}