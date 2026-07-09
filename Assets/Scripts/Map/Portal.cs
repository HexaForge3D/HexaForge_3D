using UnityEngine;
using UnityEngine.InputSystem;

public class Portal : MonoBehaviour
{
    private bool _isPlayerInCollider;

    public static event System.Action<Portal> OnPortalInteracted;

    private void OnEnable()
    {
        PlayerInputSystem.OnInteract += Handleinteraction;
    }

    private void OnDisable()
    {
        PlayerInputSystem.OnInteract -= Handleinteraction;
    }

    private void Handleinteraction()
    {
        if (_isPlayerInCollider == false)
        {
            return;
        }

        OnPortalInteracted?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (MapManager.Instance != null)
            {
                _isPlayerInCollider = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (MapManager.Instance != null)
            {
                _isPlayerInCollider = false;
            }
        }
    }
}