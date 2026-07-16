using UnityEngine;

public enum PortalType : byte
{
    None,
    Dungeon,
    Store,
    MainQuest,
    Smithy
}

public class Portal : MonoBehaviour
{
    private bool _isPlayerInCollider;

    public static event System.Action<Portal> OnPortalInteracted;

    [SerializeField] private PortalType _portalType;

    public PortalType PortalType => _portalType;

    public Portal _partnerPortal { get; set; }


    private void OnEnable()
    {
        PortalManager.Instance.RegisterPortal(this);
        PlayerInputSystem.OnInteract += Handleinteraction;
    }

    private void OnDisable()
    {
        PortalManager.Instance.UnRegisterPortal(this);
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
                MapManager.Instance.SetPlayer(other.transform);
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