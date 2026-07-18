using System;
using UnityEngine;

public enum NPCId : byte
{
    None,
    Store,
    MainQuest,
    Smithy
}

public class NPC : MonoBehaviour
{
    private bool _isPlayerInCollider;

    public static event Action<NPC> OnNPCInteracted;

    [SerializeField] private NPCId _npcId;

    public NPCId NPCId
    {
        get
        {
            return _npcId;
        }
    }

    private void OnEnable()
    {
        PlayerInputSystem.OnInteract += HandleInteraction;
    }

    private void OnDisable()
    {
        PlayerInputSystem.OnInteract -= HandleInteraction;
    }

    public void RequestInteraction()
    {
        OnNPCInteracted?.Invoke(this);
    }

    private void HandleInteraction()
    {
        if (_isPlayerInCollider == false)
        {
            return;
        }

        RequestInteraction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInCollider = false;
        }
    }
}