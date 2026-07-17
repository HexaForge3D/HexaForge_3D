using System;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private PlayerController _playerController;

    // 경험치가 변화하면(증가하면) 발생하는 이벤트
    public static event Action<int, int> OnExpChanged;

    // 스킬 레벨업을 위하여 레벨이 오르면 발생하는 이벤트
    public static event Action OnLevelUp;

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

    private void HandleExpTestKey()
    {
        AddExp(100); // 경험치는 변경가능 Test용이라서
    }

    public void AddExp(int amount)
    {
        if (_playerController == null || _playerController.PlayerData == null) return;

        CharacterSaveData data = _playerController.PlayerData;

        data.Exp += amount;
        Debug.Log($"[PlayerState] 경험치 {amount} 획득! (임시 계산용 Exp: {data.Exp})");

        // 경험치가 레벨업에 필요한 경험치를 초가할 수 있으므로 호출
        CheckLevelUp();

        int requiredExp = GetRequiredExp(data.Level);
        OnExpChanged?.Invoke(data.Exp, requiredExp);
    }
    private void CheckLevelUp()
    {
        CharacterSaveData data = _playerController.PlayerData;
        int requiredExp = GetRequiredExp(data.Level);

        // 연속으로 레벨업 처리
        while (data.Exp >= requiredExp)
        {
            data.Exp -= requiredExp; // 필요 경험치만큼 차감
            data.Level++;            // 레벨 1 증가

            Debug.Log($"<color=purple>레벨 업! (현재 레벨: {data.Level})</color>");

            // 레벨업 이벤트 발생 스킬 레벨을 올리기 위하여
            OnLevelUp?.Invoke();

            requiredExp = GetRequiredExp(data.Level);
        }
    }

    private int GetRequiredExp(int level)
    {
        int x = level;
        
        int y = 0;
        
        if (x >= 10)
        {
            y = x - 9;
        }

        int requiredExp = (x * 200) + (y * 100);

        return requiredExp;
    }

}