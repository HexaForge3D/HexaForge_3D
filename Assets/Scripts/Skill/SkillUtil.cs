using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillUtil : MonoBehaviour
{
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

    private void Awake()
    {
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

        if (playerData.Mp < skillData.ManaCost)
        {
            Debug.Log("마나가 부족합니다!!");

            OnLackMana?.Invoke(skillData.ID);
            return;
        }

        playerData.Mp -= skillData.ManaCost;
        _skillCoolTime[skillData.ID] = Time.time + skillData.CoolDown;

        OnSkillSuccess?.Invoke(skillData);
        OnSkillCoolTimeStart?.Invoke(skillData.ID, skillData.CoolDown);
        Debug.Log($"<color=cyan>[SkillUtil] {skillData.Name} 발동!</color> 데미지: {skillData.Damage}, 남은 마나: {playerData.Mp}");
    }
}