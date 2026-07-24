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

    protected virtual void OnEnable()
    {
        IsInDungeon = true;
        PlayerBattle.OnPlayerDead += HandlePlayerDead;
    }

    protected virtual void OnDisable()
    {
        IsInDungeon = false;
        PlayerBattle.OnPlayerDead -= HandlePlayerDead;  
    }

    private void HandlePlayerDead()
    {
        InvokeFailed(DungeonFailReason.PlayerDead);
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
