using UnityEngine;

public abstract class InteractObject : MonoBehaviour
{
    [Header("상호작용 대상")]
    [SerializeField] protected string _playerTag = "Player";

    protected bool _isPlayerInCollider = false;
    protected bool _isInteractRequested = false;

    protected virtual void OnEnable()
    {
        PlayerInputSystem.OnInteract += HandleInteractionInternal;
    }

    protected virtual void OnDisable()
    {
        PlayerInputSystem.OnInteract -= HandleInteractionInternal;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _isPlayerInCollider = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _isPlayerInCollider = false;
        }
    }

    private void HandleInteractionInternal()
    {
        if (!_isPlayerInCollider || _isInteractRequested || !CanInteract())
        {
            return;
        }

        _isInteractRequested = true;
        OnInteract();
    }

    protected virtual bool CanInteract()
    {
        return true;
    }

    protected abstract void OnInteract();
}