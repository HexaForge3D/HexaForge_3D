using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnPair
{
    public Transform _spawnPoint;
    public GameObject _monsterPrefab;
}

public class DefenceFieldManager : BaseDungeonController
{
    [SerializeField] private SpawnPair[] _spawnPairs;
    [SerializeField] private GameObject _defenceTarget;
    [SerializeField] private Transform _monsterGroup;
    [SerializeField] private int _waveCount = 5;
    [SerializeField] private float _countdownDuration = 10f;

    public static event Action OnClearField;
    public static event Action OnFailField;

    public static event Action<int, int> OnWaveChanged;
    public static event Action<float> OnCountdownChanged;

    private bool _isFailed = false;
    private bool _isStarted = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        DefenceTarget.OnTargetDestroyed += HandleTargetDestroyed;
        DefenceTarget.OnDefenceStartRequested += HandleDefenceStartRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        DefenceTarget.OnTargetDestroyed -= HandleTargetDestroyed;
        DefenceTarget.OnDefenceStartRequested -= HandleDefenceStartRequested;
    }

    private void HandleDefenceStartRequested()
    {
        if (_isStarted)
        {
            return;
        }

        _isStarted = true;
        Debug.Log("방어 목표 상호작용 감지: 디펜스 시퀀스를 시작합니다.");
        StartDefenceSequence().Forget();
    }

    private void HandleTargetDestroyed()
    {
        _isFailed = true;
        Debug.Log("방어 목표가 파괴되어 디펜스 필드가 실패 처리되었습니다.");

        OnFailField?.Invoke();
        InvokeFailed(DungeonFailReason.NpcDead);
    }

    private async UniTask StartDefenceSequence()
    {
        Debug.Log($"{_countdownDuration}초 후 디펜스 시작");

        await CountdownAsync(_countdownDuration);

        for (int i = 0; i < _waveCount; i++)
        {
            if (_isFailed || _defenceTarget == null)
            {
                Debug.Log("목표가 파괴되었습니다. 실패!");
                return;
            }

            OnWaveChanged?.Invoke(i + 1, _waveCount);

            SpawnAllDefinedMonsters();
            Debug.Log($"{i + 1} 웨이브 완료");

            float nextWaveDelay = _countdownDuration * 1.5f;
            Debug.Log($"{i + 2} 웨이브 {(_countdownDuration * 2 )}초뒤 시작");
            await CountdownAsync(nextWaveDelay);
        }

        if (!_isFailed && _defenceTarget != null)
        {

            Debug.Log("모든 웨이브 클리어!");

            DungeonReward reward = new DungeonReward
            {
                Gold = 100,
                ItemIds = new List<string>()
            };

            OnClearField?.Invoke();
            InvokeCleared(reward);
        }
    }

    private async UniTask CountdownAsync(float duration)
    {
        float remaining = duration;

        while (remaining > 0f)
        {
            OnCountdownChanged?.Invoke(remaining);
            await UniTask.Delay(1000);
            remaining -= 1f;
        }

        OnCountdownChanged?.Invoke(0f);
    }

    private void SpawnAllDefinedMonsters()
    {
        foreach (SpawnPair pair in _spawnPairs)
        {
            if (pair._spawnPoint != null && pair._monsterPrefab != null)
            {
                Instantiate(pair._monsterPrefab, pair._spawnPoint.position, pair._spawnPoint.rotation, _monsterGroup);
            }
        }
    }
}