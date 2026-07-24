using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCEscortFieldManager : BaseDungeonController
{
    [SerializeField] private NPCPatrolController _npcPatrolController;
    [SerializeField] public GameObject _itemGroup;

    public static event Action OnClearField;
    public static event Action OnFailField;
    public static event Action<string> OnStartField;

    private bool _isFailed = false;

    private bool _isCheatClear = false;
    private bool _isCheatFail = false;

    public static NPCEscortFieldManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        NPCPatrolController.OnPatrolFinished += HandlePatrolFinished;

        PlayerInputSystem.OnCheatDungeonCleared += HandleCheatClear;
        PlayerInputSystem.OnCheatDungeonFailed += HandleCheatFail;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        NPCPatrolController.OnPatrolFinished -= HandlePatrolFinished;

        PlayerInputSystem.OnCheatDungeonCleared -= HandleCheatClear;
        PlayerInputSystem.OnCheatDungeonFailed -= HandleCheatFail;
    }

    private void Start()
    {
        OnStartField?.Invoke("NPCEscortBGM");
    }

    private void HandleCheatClear()
    {
        if (_isCheatClear || _isFailed)
        {
            return;
        }

        _isCheatClear = true;
        Debug.Log("[NPCEscortFieldManager] 치트키: 던전 강제 클리어");
        ClearDungeon();
    }

    private void HandleCheatFail()
    {
        if (_isCheatFail || _isFailed)
        {
            return;
        }

        _isCheatFail = true;
        Debug.Log("[NPCEscortFieldManager] 치트키: 던전 강제 실패");
        FailDungeon();
    }

    private void HandlePatrolFinished()
    {
        Debug.Log("[NPCEscortFieldManager 필드 클리어 조건 달성");

        ClearDungeon();
    }

    private void ClearDungeon()
    {
        DungeonReward reward = CreateReward();

        OnClearField?.Invoke();
        InvokeCleared(reward);
    }

    private void FailDungeon()
    {
        _isFailed = true;
        OnFailField?.Invoke();
        InvokeFailed(DungeonFailReason.NpcDead);
    }
}