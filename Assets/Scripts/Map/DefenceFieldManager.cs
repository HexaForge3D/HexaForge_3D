using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

[Serializable]
public class SpawnPair
{
    public Transform _spawnPoint;
    public GameObject _monsterPrefab;
}

public class DefenceFieldManager : BaseDungeonController
{
    [Header("몬스터 및 아이템 스폰루트")]
    [SerializeField] private Transform _monsterGroup;
    [SerializeField] public GameObject _itemGroup;

    [SerializeField] private SpawnPair[] _spawnPairs;
    [SerializeField] private GameObject _defenceTarget;
    [SerializeField] private int _waveCount = 5;
    [SerializeField] private float _countdownDuration = 10f;

    [Header("시작 포탈/오브젝트 설정")]
    [SerializeField] private GameObject _startPortalObject;

    public static event Action OnClearField;
    public static event Action OnFailField;

    public static event Action<int, int> OnWaveChanged;
    public static event Action<float> OnCountdownChanged;
    public static event Action<string> OnStartField;

    private bool _isStarted = false;

    public static DefenceFieldManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        DefenceTarget.OnTargetDestroyed += HandleTargetDestroyed;
        DefenceTarget.OnDefenceStartRequested += HandleDefenceStartRequested;

        OnDungeonFailed += HandleDungeonFailedEvent;
    }

    public void Start()
    {
        OnStartField?.Invoke("DefenceBGM");

        if (_startPortalObject != null)
        {
            _startPortalObject.SetActive(true);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        DefenceTarget.OnTargetDestroyed -= HandleTargetDestroyed;
        DefenceTarget.OnDefenceStartRequested -= HandleDefenceStartRequested;

        OnDungeonFailed -= HandleDungeonFailedEvent;
    }

    private void HandleDefenceStartRequested()
    {
        if (_isStarted)
        {
            return;
        }

        _isStarted = true;
        Debug.Log("방어 목표 상호작용 감지: 디펜스 시퀀스를 시작합니다.");

        if (_startPortalObject != null)
        {
            _startPortalObject.SetActive(false);
        }

        StartDefenceSequence().Forget();

        OnStartField?.Invoke("DefenceWaveStartBGM");
    }

    private void HandleTargetDestroyed()
    {
        _isFailed = true;
        Debug.Log("방어 목표가 파괴되어 디펜스 필드가 실패 처리되었습니다.");

        FailDungeon();
    }

    private void HandleDungeonFailedEvent(DungeonFailReason reason)
    {
        if (_isFailed)
        {
            return;
        }

        _isFailed = true;
        Debug.Log($"[DefenceFieldManager] 던전 실패 (사유: {reason}). 디펜스 시퀀스를 중단합니다.");
    }

    protected override void OnCheatClear()
    {
        Debug.Log("[DefenceFieldManager] 치트키: 던전 클리어");
        ClearDungeon();
    }

    protected override void OnCheatFail()
    {
        Debug.Log("[DefenceFieldManager] 치트키: 던전 실패");
        FailDungeon();
    }

    private async UniTask StartDefenceSequence()
    {
        Debug.Log($"{_countdownDuration}초 후 디펜스 시작");

        bool isCountdownFinished = await CountdownAsync(_countdownDuration);
        if (!isCountdownFinished) return;

        for (int i = 0; i < _waveCount; i++)
        {
            if (_isCheatClear)
            {
                ClearDungeon();
                return;
            }

            if (_isCheatFail)
            {
                FailDungeon();
                return;
            }

            if (_isFailed || _defenceTarget == null)
            {
                Debug.Log("목표가 파괴되었습니다. 실패!");
                return;
            }

            OnWaveChanged?.Invoke(i + 1, _waveCount);

            SpawnAllDefinedMonsters();
            Debug.Log($"{i + 1} 웨이브 완료");

            float nextWaveDelay = _countdownDuration;
            Debug.Log($"{i + 2} 웨이브 {_countdownDuration}초 뒤 시작");

            await CountdownAsync(nextWaveDelay);
        }

        if (_isCheatClear)
        {
            ClearDungeon();
            return;
        }

        if (_isCheatFail)
        {
            FailDungeon();
            return;
        }

        if (!_isFailed && _defenceTarget != null)
        {
            Debug.Log("모든 웨이브 클리어!");

            ClearDungeon();
        }
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

    private void ClearDungeon()
    {
        OnClearField?.Invoke();
        ClearDungeonCommon();
    }

    private void FailDungeon()
    {
        OnFailField?.Invoke();
        FailDungeonCommon(DungeonFailReason.NpcDead);
    }

    private async UniTask<bool> CountdownAsync(float duration)
    {
        float remaining = duration;

        while (remaining > 0f)
        {
            if (_isCheatClear)
            {
                ClearDungeon();
                return false;
            }

            if (_isCheatFail || _isFailed)
            {
                FailDungeon();
                return false;
            }

            OnCountdownChanged?.Invoke(remaining);

            await UniTask.Delay(1000);

            remaining -= 1f;
        }

        OnCountdownChanged?.Invoke(0f);
        return true;
    }
}