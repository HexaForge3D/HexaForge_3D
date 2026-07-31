using System;
using UnityEngine;

public class BossFieldNPC : InteractObject
{
    [SerializeField] private float _detectRadius = 3f;

    public static event Action OnBossFieldStartRequested;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnInteract()
    {
        Debug.Log("<color=green>[BossFieldNPC] 플레이어 상호작용! 보스전 시작 요청.");
        OnBossFieldStartRequested?.Invoke();
        this.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectRadius);
    }
}