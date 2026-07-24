using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossSpawnPair
{
    [SerializeField] private GameObject _monsterPrefab;
    public GameObject MonsterPrefab => _monsterPrefab;
}

public class BossFieldManager : BaseDungeonController
{


    [Header("보스 설정")]
    [SerializeField] private GameObject _dummyBossObejct;
    [SerializeField] private Transform _bossSpawnPoint;
    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private float _countdownDuration = 5f;

    [Header("소환 반경 및 부모 설정")]
    [SerializeField] private float _spawnRadius = 5f;
    [SerializeField] private Transform _monsterGroup;

    [Header("체력 구간별 소환 그룹 및 소환 포인트")]
    [SerializeField] private Transform _group1SpawnPoint;
    [SerializeField] private List<BossSpawnPair> _group1SpawnPairs;

    [SerializeField] private Transform _group2SpawnPoint;
    [SerializeField] private List<BossSpawnPair> _group2SpawnPairs;

    [SerializeField] private Transform _group3SpawnPoint;
    [SerializeField] private List<BossSpawnPair> _group3SpawnPairs;

    public static event Action OnBattleBossStartField;
    public static event Action OnClearField;
    public static event Action OnFailField;
    public static event Action<string> OnStartField;
    public static event Action<float> OnCountdownChanged;
    public static event Action<int, int> OnBossHpChanged;

    private GameObject _spawnedBoss;
    private BossMonsterHealth _bossHealth;

    private bool _isStarted = false;
    private bool _isCleared = false;
    private bool _isFailed = false;

    private bool _isSpawnGroup1 = false;
    private bool _isSpawnGroup2 = false;
    private bool _isSpawnGroup3 = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        BossFieldNPC.OnBossFieldStartRequested += HandleInteractionRequested;

        PlayerInputSystem.OnCheatDungeonCleared += HandleCheatClear;
        PlayerInputSystem.OnCheatDungeonFailed += HandleCheatFail;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        BossFieldNPC.OnBossFieldStartRequested -= HandleInteractionRequested;

        PlayerInputSystem.OnCheatDungeonCleared -= HandleCheatClear;
        PlayerInputSystem.OnCheatDungeonFailed -= HandleCheatFail;

        UnsubscribeBossEvents();
    }

    private void Start()
    {
        OnStartField?.Invoke("BossBGM");
        Debug.Log("<color=cyan>[BossFieldManager] 보스 대기 중... NPC와 상호작용하여 전투를 시작하세요.</color>");

        if (_dummyBossObejct !=  null)
        {
            _dummyBossObejct.SetActive(true);
        }
    }

    private void HandleInteractionRequested()
    {
        if (_isStarted || _isCleared || _isFailed)
        {
            return;
        }

        _isStarted = true;
        Debug.Log("<color=green>[BossFieldManager] 상호작용 감지! 5초 후 보스전이 시작됩니다.</color>");
        StartBossFieldSequence().Forget();
    }

    private async UniTaskVoid StartBossFieldSequence()
    {
        OnBattleBossStartField?.Invoke();

        float remainingTime = _countdownDuration;
        while (remainingTime > 0f)
        {
            if (_isCleared || _isFailed)
            {
                return;
            }

            OnCountdownChanged?.Invoke(remainingTime);
            await UniTask.Delay(1000);
            remainingTime -= 1f;
        }
        OnCountdownChanged?.Invoke(0f);

        SpawnBoss();
    }

    private void SpawnBoss()
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        if (_spawnedBoss != null)
        {
            Debug.LogWarning("[BossFieldManager] 보스가 이미 존재합니다!");
            return;
        }

        if (_bossPrefab == null || _bossSpawnPoint == null)
        {
            Debug.LogError("[BossFieldManager] 보스 프리팹 또는 스폰 포인트가 설정되지 않았습니다!");
            return;
        }

        if (_dummyBossObejct != null)
        {
            Destroy(_dummyBossObejct);
        }

        _spawnedBoss = Instantiate(_bossPrefab, _bossSpawnPoint.position, _bossSpawnPoint.rotation);

        if (_monsterGroup != null)
        {
            _spawnedBoss.transform.SetParent(_monsterGroup);
        }

        _bossHealth = _spawnedBoss.GetComponent<BossMonsterHealth>();

        if (_bossHealth != null)
        {
            _bossHealth.OnBossHpChanged += HandleBossHpChanged;
            _bossHealth.OnBossMonsterDieCount += HandleBossDied;

            OnBossHpChanged?.Invoke(_bossHealth._maxHealth, _bossHealth._maxHealth);
        }
        else
        {
            Debug.LogError("[BossFieldManager] 스폰된 보스 프리팹에 BossMonsterHealth 컴포넌트가 없습니다!");
        }

        Debug.Log("<color=yellow>[BossFieldManager] 보스가 등장했습니다! 전투 시작!</color>");
    }

    private void HandleBossHpChanged(int currentHp, int maxHp)
    {
        OnBossHpChanged?.Invoke(currentHp, maxHp);

        float hpPercentage = (float)currentHp / maxHp * 100f;

        if (hpPercentage <= 75f && !_isSpawnGroup1)
        {
            _isSpawnGroup1 = true;
            SpawnMonsterGroup(_group1SpawnPoint, _group1SpawnPairs, "그룹 1 (75% 구간)");
        }

        if (hpPercentage <= 50f && !_isSpawnGroup2)
        {
            _isSpawnGroup2 = true;
            SpawnMonsterGroup(_group2SpawnPoint, _group2SpawnPairs, "그룹 2 (50% 구간)");
        }

        if (hpPercentage <= 25f && !_isSpawnGroup3)
        {
            _isSpawnGroup3 = true;
            SpawnMonsterGroup(_group3SpawnPoint, _group3SpawnPairs, "그룹 3 (25% 구간)");
        }
    }

    private void SpawnMonsterGroup(Transform baseSpawnPoint, List<BossSpawnPair> spawnPairs, string groupName)
    {
        if (spawnPairs == null || spawnPairs.Count == 0 || baseSpawnPoint == null)
        {
            Debug.LogWarning($"[BossFieldManager] {groupName} 소환 데이터 또는 기준 스폰 포인트가 누락되었습니다.");
            return;
        }

        Debug.Log($"<color=orange>[BossFieldManager] 보스 체력 조건 달성! {groupName} 몬스터들을 소환합니다.</color>");

        foreach (var pair in spawnPairs)
        {
            if (pair.MonsterPrefab != null)
            {
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * _spawnRadius;
                Vector3 spawnPosition = baseSpawnPoint.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

                GameObject monsterInstance = Instantiate(pair.MonsterPrefab, spawnPosition, baseSpawnPoint.rotation);
                if (_monsterGroup != null)
                {
                    monsterInstance.transform.SetParent(_monsterGroup);
                }
            }
        }
    }

    private void HandleBossDied(BossMonsterHealth deadBoss)
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        _isCleared = true;
        Debug.Log("<color=green>[BossFieldManager] 보스를 처치했습니다! 던전 클리어!</color>");

        ClearDungeon();
    }

    private void UnsubscribeBossEvents()
    {
        if (_bossHealth != null)
        {
            _bossHealth.OnBossHpChanged -= HandleBossHpChanged;
            _bossHealth.OnBossMonsterDieCount -= HandleBossDied;
        }
    }

    private void HandleCheatClear()
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        _isCleared = true;
        Debug.Log("[BossFieldManager] 치트키: 던전 클리어");
        ClearDungeon();
    }

    private void HandleCheatFail()
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        _isFailed = true;
        Debug.Log("[BossFieldManager] 치트키: 던전 실패");
        FailDungeon();
    }

    private void ClearDungeon()
    {
        DungeonReward reward = CreateReward();

        OnClearField?.Invoke();
        InvokeCleared(reward);
    }

    private void FailDungeon()
    {
        UnsubscribeBossEvents();

        OnFailField?.Invoke();
        InvokeFailed(DungeonFailReason.PlayerDead);
    }
}