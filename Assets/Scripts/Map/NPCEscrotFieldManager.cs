using UnityEngine;
using System;

public class NPCEscortFieldManager : BaseDungeonController
{
    [Header("호위목표NPC")]
    [SerializeField] private NPCPatrolController _npcPatrolController;

    [Header("시작 포탈/오브젝트 설정")]
    [SerializeField] private GameObject _startPortalObject;

    [Header("몬스터 및 아이템 스폰루트")]
    [SerializeField] private Transform _monsterGroup;
    [SerializeField] public GameObject _itemGroup;

    public static event Action OnClearField;
    public static event Action OnFailField;
    public static event Action<string> OnStartField;

    private bool _isStarted = false;

    public static NPCEscortFieldManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        NPCPatrolController.OnPatrolFinished += HandlePatrolFinished;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        NPCPatrolController.OnPatrolFinished -= HandlePatrolFinished;
    }

    private void Start()
    {
        OnStartField?.Invoke("NPCEscortBGM");

        if (_startPortalObject != null)
        {
            _startPortalObject.SetActive(true);
        }
    }
    public void HandleEscortStart()
    {
        if (_isStarted || _isFailed)
        {
            return;
        }

        _isStarted = true;

        if (_startPortalObject != null)
        {
            _startPortalObject.SetActive(false);
        }
    }

    protected override void OnCheatClear()
    {
        Debug.Log("[NPCEscortFieldManager] 치트키: 던전 강제 클리어");
        ClearDungeon();
    }

    protected override void OnCheatFail()
    {
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
        OnClearField?.Invoke();
        ClearDungeonCommon();
    }

    private void FailDungeon()
    {
        OnFailField?.Invoke();
        FailDungeonCommon(DungeonFailReason.PlayerDead);
    }
}