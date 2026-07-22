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
public class DungeonReward
{
    public int Gold;
    public List<string> ItemIds;
}

public abstract class BaseDungeonController : MonoBehaviour
{
    public static event Action<DungeonReward> OnDungeonCleared;
    public static event Action<DungeonFailReason> OnDungeonFailed;

    public static bool IsInDungeon { get; private set; }

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
    }

    protected static void InvokeFailed(DungeonFailReason reason)
    {
        Debug.Log($"[BaseDungeonController] 던전 실패. 사유: {reason}");
        OnDungeonFailed?.Invoke(reason);
    }
}
