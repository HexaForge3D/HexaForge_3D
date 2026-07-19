using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private PlayerController _playerController;

    // 경험치가 변화하면(증가하면) 발생하는 이벤트
    public static event Action<int, int> OnExpChanged;

    // 스킬 레벨업을 위하여 레벨이 오르면 발생하는 이벤트
    public static event Action OnLevelUp;

    // 마나가 변화하면(회복되면) 발생하는 이벤트
    public static event Action<int, int> OnMpChanged;

    private const int MaxLevel = 100; // 최대레벨은 100으로 수정불가능 하게
    private const int MaxExp = 1700000; // 전체 경험치는 1,700,000으로 설정 => 1 -> 2 하는 걸 보기 위해서
    private const float ExpCurve = 0.5f; // 곡선으로 레벨이 올라갈 수록 레벨 업하기 어렵게 수정 => 숫자가 커질 수록 후반부에 편해지지만, 0.5f 기본세팅함

    private int _maxMp;
    private bool _isMpSet = false;
    private CancellationTokenSource _cts;

    private const float ManaRegenTimer = 1f;
    private const float ManaRegenRate = 0.05f;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        _cts = new CancellationTokenSource();
        ManaRegen(_cts.Token).Forget();
    }

    private void Update()
    {
        if (_isMpSet == false && _playerController != null && _playerController.PlayerData != null)
        {
            _maxMp = _playerController.PlayerData.Mp;
            _isMpSet = true;
        }
    }

    private void OnEnable()
    {
        PlayerInputSystem.OnExpTest += HandleExpTestKey;
        PlayerBattle.OnPlayerDead += CancelToken;
    }

    private void OnDisable()
    {
        PlayerInputSystem.OnExpTest -= HandleExpTestKey;
        PlayerBattle.OnPlayerDead -= CancelToken;
    }

    private void CancelToken()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
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

    private void OnDestroy()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
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

    private async UniTaskVoid ManaRegen(CancellationToken token)
    {
        try
        {
            while (token.IsCancellationRequested == false)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(ManaRegenTimer), cancellationToken: token);

                if (_isMpSet == false || _playerController.PlayerData == null) continue;

                CharacterSaveData data = _playerController.PlayerData;

                if (data.CurrentMp < _maxMp)
                {
                    int regenAmount = Mathf.Max(1, Mathf.FloorToInt(_maxMp * ManaRegenRate));

                    data.CurrentMp += regenAmount;

                    if (data.CurrentMp > _maxMp)
                    {
                        data.CurrentMp = _maxMp;
                    }
                    Debug.Log($"<color=blue>마나가 {regenAmount} 회복되었습니다. 현재마나: {data.Mp} / 최대마나: {_maxMp}</color>");

                    OnMpChanged?.Invoke(data.CurrentMp, _maxMp);
                }
            }
        }

        catch (OperationCanceledException)
        {

        }
    }

}