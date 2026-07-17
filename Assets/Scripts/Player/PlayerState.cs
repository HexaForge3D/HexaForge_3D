using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private PlayerController _playerController;

    // 경험치가 변화하면(증가하면) 발생하는 이벤트
    public static event Action<int, int> OnExpChanged;

    // 스킬 레벨업을 위하여 레벨이 오르면 발생하는 이벤트
    public static event Action OnLevelUp;

    private const int MaxLevel = 100; // 최대레벨은 100으로 수정불가능 하게
    private const int MaxExp = 80000; // 전체 경험치는 80000으로 설정

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        PlayerInputSystem.OnExpTest += HandleExpTestKey;
    }

    private void OnDisable()
    {
        PlayerInputSystem.OnExpTest -= HandleExpTestKey;
    }

    public int LevelFromExp(int totalExp)
    {
        if (totalExp >= MaxExp)
        {
            return MaxLevel;
        }

        float ratio = (float)totalExp / MaxExp;

        int calculatedLevel = 1 + Mathf.FloorToInt(ratio * (MaxLevel - 1));

        return calculatedLevel;
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

        Debug.Log($"[PlayerState] 경험치 {amount} 획득! (총 누적 Exp: {data.Exp} / {MaxExp})");

        int levelAfter = LevelFromExp(data.Exp);

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