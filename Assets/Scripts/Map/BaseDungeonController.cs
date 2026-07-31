using System;
using System.Collections.Generic;
using UnityEngine;

public enum DungeonFailReason
{
    None,
    PlayerDead,
    NpcDead
}

[Serializable]
public class RewardItem
{
    public string ItemId;
    public int Count = 1;
}

[Serializable]
public class DungeonReward
{
    public int Gold;
    public List<RewardItem> Items;
}

public abstract class BaseDungeonController : MonoBehaviour
{
    public static event Action<DungeonReward> OnDungeonCleared;
    public static event Action<DungeonFailReason> OnDungeonFailed;

    public static bool IsInDungeon { get; private set; }

    [Header("Dungeon Reward")]
    [SerializeField] private int _rewardGold = 100;
    [SerializeField] private List<RewardItem> _rewardItems = new List<RewardItem>();

    protected bool _isCleared = false;
    protected bool _isFailed = false;
    protected bool _isCheatClear = false;
    protected bool _isCheatFail = false;

    protected virtual void OnEnable()
    {
        IsInDungeon = true;
        PlayerBattle.OnPlayerDead += HandlePlayerDead;

        PlayerInputSystem.OnCheatDungeonCleared += HandleCheatClearInternal;
        PlayerInputSystem.OnCheatDungeonFailed += HandleCheatFailInternal;
    }

    protected virtual void OnDisable()
    {
        IsInDungeon = false;
        PlayerBattle.OnPlayerDead -= HandlePlayerDead;

        PlayerInputSystem.OnCheatDungeonCleared -= HandleCheatClearInternal;
        PlayerInputSystem.OnCheatDungeonFailed -= HandleCheatFailInternal;
    }

    private void HandlePlayerDead()
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        _isFailed = true;
        InvokeFailed(DungeonFailReason.PlayerDead);
    }

    private void HandleCheatClearInternal()
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        _isCheatClear = true;
        Debug.Log($"[{GetType().Name}] 치트키: 던전 강제 클리어");
        OnCheatClear();
    }

    private void HandleCheatFailInternal()
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        _isCheatFail = true;
        Debug.Log($"[{GetType().Name}] 치트키: 던전 강제 실패");
        OnCheatFail();
    }

    protected virtual void OnCheatClear()
    {
        ClearDungeonCommon();
    }

    protected virtual void OnCheatFail()
    {
        FailDungeonCommon(DungeonFailReason.PlayerDead);
    }

    protected void ClearDungeonCommon()
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        _isCleared = true;
        DungeonReward reward = CreateReward();
        InvokeCleared(reward);
    }

    protected void FailDungeonCommon(DungeonFailReason reason)
    {
        if (_isCleared || _isFailed)
        {
            return;
        }

        _isFailed = true;
        InvokeFailed(reason);
    }

    protected static void InvokeCleared(DungeonReward reward)
    {
        Debug.Log($"[BaseDungeonController] 던전 클리어. 보상: {reward.Gold}");
        OnDungeonCleared?.Invoke(reward);
        SoundManager.Instance.PlayUISound("Dungeon_Clear_Sound");
    }

    protected static void InvokeFailed(DungeonFailReason reason)
    {
        Debug.Log($"[BaseDungeonController] 던전 실패. 사유: {reason}");
        OnDungeonFailed?.Invoke(reason);
        SoundManager.Instance.PlayUISound("Dungeon_Failed_Sound");
    }

    protected DungeonReward CreateReward()
    {
        Debug.Log($"[BaseDungeonController] CreateReward - Gold: {_rewardGold}, ItemIds.Count: {_rewardItems.Count}");
        return new DungeonReward
        {
            Gold = _rewardGold,
            Items = new List<RewardItem>(_rewardItems)
        };
    }
}
