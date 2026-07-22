using System;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class RoomFieldManager : BaseDungeonController
{ 
    [SerializeField] private Transform _monsterGroupParent;
    [SerializeField] private GameObject _clearPortal;

    public static event Action OnClearField;
    public static event Action<int, int> OnCurrentMonsterCountChanged;

    private List<MonsterHealth> _activeMonsters = new List<MonsterHealth>();
    private int _totalMonsterCount = 0;
    private int _currentMonsterCount = 0;
    private bool _isCleared = false;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void Start()
    {
        if (_clearPortal != null)
        {
            _clearPortal.SetActive(false);
        }

        InitializeRoomMonsters();
    }

    private void InitializeRoomMonsters()
    {
        if (_monsterGroupParent == null)
        {
            Debug.LogWarning("몬스터 그룹 부모 오브젝트(_monsterGroupParent)가 지정되지 않았습니다.");
            return;
        }

        MonsterHealth[] foundMonsters = _monsterGroupParent.GetComponentsInChildren<MonsterHealth>(true);

        if (foundMonsters == null || foundMonsters.Length == 0)
        {
            Debug.LogWarning("몬스터 그룹 내부에 몬스터(MonsterHealth)가 존재하지 않습니다.");
            return;
        }

        _activeMonsters.Clear();

        foreach (var monster in foundMonsters)
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
        if (_isCleared)
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

    private void HandleFieldClear()
    {
        if (_isCleared)
        {
            return;
        }

        _isCleared = true;
        Debug.Log("룸 필드의 모든 몬스터를 처치했습니다! 필드 클리어!");

        if (_clearPortal != null)
        {
            _clearPortal.SetActive(true);
        }

        DungeonReward reward = new DungeonReward
        {
            Gold = 100,
            ItemIds = new List<string>()
        };

        OnClearField?.Invoke();
        InvokeCleared(reward);
    }
}