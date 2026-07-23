using System;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class RoomFieldManager : BaseDungeonController
{ 
    [SerializeField] private Transform _monsterGroupParent;

    public static event Action OnClearField;
    public static event Action OnFailField;
    public static event Action<int, int> OnCurrentMonsterCountChanged;
    public static event Action<bool> OnStartField;

    private List<MonsterHealth> _activeMonsters = new List<MonsterHealth>();
    private int _totalMonsterCount = 0;
    private int _currentMonsterCount = 0;
    private bool _isCleared = false;
    private bool _isFailed = false;

    private bool _isCheatClear = false;
    private bool _isCheatFail = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        PlayerInputSystem.OnCheatDungeonCleared += HandleCheatClear;
        PlayerInputSystem.OnCheatDungeonFailed += HandleCheatFail;

    }

    protected override void OnDisable()
    {
        base.OnDisable();

        PlayerInputSystem.OnCheatDungeonCleared -= HandleCheatClear;
        PlayerInputSystem.OnCheatDungeonFailed -= HandleCheatFail;
    }

    private void Start()
    {
        InitializeRoomMonsters();
        OnStartField?.Invoke(true);
    }

    private void InitializeRoomMonsters()
    {
        if (_monsterGroupParent == null)
        {
            Debug.LogWarning("[RoomFieldManager] 부모 오브젝트(_monsterGroupParent)가 지정되지 않았습니다.");
            return;
        }

        MonsterHealth[] foundMonsters = _monsterGroupParent.GetComponentsInChildren<MonsterHealth>(true);

        if (foundMonsters == null || foundMonsters.Length == 0)
        {
            Debug.LogWarning("[RoomFieldManager] 몬스터에 (MonsterHealth)가 존재하지 않습니다.");
            return;
        }

        _activeMonsters.Clear();

        foreach (MonsterHealth monster in foundMonsters)
        {
            if (monster != null)
            {
                monster.OnMonsterDieCount += HandleMonsterDie;
                _activeMonsters.Add(monster);
            }
        }

        _totalMonsterCount = _activeMonsters.Count;
        _currentMonsterCount = _totalMonsterCount;

        OnCurrentMonsterCountChanged?.Invoke(_currentMonsterCount, _totalMonsterCount);

        Debug.Log($"룸 필드 시작: 총 {_totalMonsterCount}마리의 몬스터가 감지되었습니다.");
    }

    private void HandleMonsterDie(MonsterHealth deadMonster)
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        deadMonster.OnMonsterDieCount -= HandleMonsterDie;

        if (_activeMonsters.Contains(deadMonster))
        {
            _activeMonsters.Remove(deadMonster);
        }

        _currentMonsterCount = Mathf.Max(0, _currentMonsterCount - 1);

        OnCurrentMonsterCountChanged?.Invoke(_currentMonsterCount, _totalMonsterCount);

        Debug.Log($"몬스터 처치! 남은 몬스터 수: {_currentMonsterCount} / {_totalMonsterCount}");

        if (_currentMonsterCount <= 0 || _activeMonsters.Count == 0)
        {
            HandleFieldClear();
        }
    }

    private void HandleCheatClear()
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        _isCheatClear = true;
        Debug.Log("[RoomFieldManager] 치트키: 던전 클리어");
        ClearDungeon();
    }

    private void HandleCheatFail()
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        _isCheatFail = true;
        Debug.Log("[RoomFieldManager] 치트키: 던전 실패");
        FailDungeon();
    }

    private void HandleFieldClear()
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        _isCleared = true;
        Debug.Log("룸 필드의 모든 몬스터를 처치했습니다! 필드 클리어!");

        ClearDungeon();
    }

    //private void HandleFieldFail()
    //{
    //    if (_isCleared || _isFailed)
    //    {
    //        return;
    //    }

    //    _isFailed = true;
    //    Debug.Log("룸 필드 클리어 실패");

    //    FailDungeon();
    //}

    private void ClearDungeon()
    {
        DungeonReward reward = new DungeonReward
        {
            Gold = 100,
            ItemIds = new List<string>()
        };

        OnClearField?.Invoke();
        InvokeCleared(reward);
    }

    private void FailDungeon()
    {
        OnFailField?.Invoke();
        InvokeFailed(DungeonFailReason.PlayerDead);
    }
}