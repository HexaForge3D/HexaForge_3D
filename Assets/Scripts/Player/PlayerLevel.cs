using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    private PlayerController _playerController;

    // 경험치가 변화하면(증가하면) 발생하는 이벤트
    public static event Action<int, int> OnExpChanged;

    // 스킬 레벨업을 위하여 레벨이 오르면 발생하는 이벤트
    public static event Action OnLevelUp;

    private const int MaxLevel = 100; // 최대레벨은 100으로 수정불가능 하게
    private const int MaxExp = 1700000; // 전체 경험치는 1,700,000으로 설정 => 1 -> 2 하는 걸 보기 위해서
    private const float ExpCurve = 0.5f; // 곡선으로 레벨이 올라갈 수록 레벨 업하기 어렵게 수정 => 숫자가 커질 수록 후반부에 편해지지만, 0.5f 기본세팅함

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        PlayerInputSystem.OnExpTest += HandleExpTestKey;
        MonsterHealth.OnMonsterDied += AddExp;
    }

    private void OnDisable()
    {
        PlayerInputSystem.OnExpTest -= HandleExpTestKey;
        MonsterHealth.OnMonsterDied -= AddExp;
    }

    public static int LevelFromExp(int totalExp)
    {
        if (totalExp >= MaxExp)
        {
            return MaxLevel;
        }

        float ratio = (float)totalExp / MaxExp;
        float curveRatio = Mathf.Pow(ratio, ExpCurve);
        int calculatedLevel = 1 + Mathf.FloorToInt(curveRatio * (MaxLevel - 1));

        return calculatedLevel;
    }

    public int ExpForNextLevel(int targetLevel)
    {
        if (targetLevel <= 1) return 0;
        if (targetLevel >= MaxLevel) return MaxExp;

        float targetCurveRatio = (float)(targetLevel - 1) / (MaxLevel - 1);
        float ratio = Mathf.Pow(targetCurveRatio, 1f / ExpCurve);

        return Mathf.CeilToInt(ratio * MaxExp);
    }

    private void HandleExpTestKey()
    {
        AddExp(100);
    }

    public void AddExp(int amount)
    {
        if (_playerController == null || _playerController.PlayerData == null) return;

        CharacterSaveData data = _playerController.PlayerData;

        if (data.Exp >= MaxExp) return;

        int levelBefore = LevelFromExp(data.Exp);

        int newExp = data.Exp + amount;

        if (newExp > MaxExp) newExp = MaxExp;

        data.Exp = newExp;

        int levelAfter = LevelFromExp(data.Exp);

        if (levelAfter >= MaxLevel)
        {
            Debug.Log($"[PlayerState] 경험치 {amount} 획득! (만렙입니다 / 총 누적 Exp: {data.Exp} / {MaxExp})");
        }

        else
        {
            int nextLevelTotalExp = ExpForNextLevel(levelAfter + 1);
            int remainExp = nextLevelTotalExp - data.Exp;

            Debug.Log($"[PlayerState] 경험치 {amount} 획득! (총 누적 Exp: {data.Exp} / {MaxExp} | 다음 레벨업까지 남은 경험치: {remainExp})");
        }

        SaveManager.Instance.AddExp(data.SlotId, data.Exp, levelBefore, levelAfter);

        if (levelAfter > levelBefore)
        {
            int levelUpCount = levelAfter - levelBefore;

            for (int i = 0; i < levelUpCount; i++)
            {
                Debug.Log($"<color=purple>레벨 업! (도달 레벨: {levelBefore + i + 1})</color>");

                if (data.Skills == null) data.Skills = new SkillSaveData();

                if (data.Skills.Skills == null) data.Skills.Skills = new List<SkillProgressData>();

                OnLevelUp?.Invoke();
            }

            SkillUtil.Instance?.InvokeSkillDataUpdated();
        }
        OnExpChanged?.Invoke(data.Exp, MaxExp);
    }
}