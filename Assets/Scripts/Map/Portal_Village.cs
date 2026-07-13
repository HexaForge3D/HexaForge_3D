using UnityEngine;

public class Portal_Village : MonoBehaviour
{
    [SerializeField] private string _portalID;
    [SerializeField] private string _targetPortalID;

    private bool _isPlayerInPortal = false;

    public string PortalID()
    {
        return _portalID;
    }

    private void Start()
    {
        if (MapManager.Instance != null)
        {
            MapManager.Instance.RegisterPortal(this);
        }
    }
    private void OnEnable()
    {
        PlayerInputSystem.OnInteract += HandleInteraction;
    }

    private void OnDisable()
    {
        PlayerInputSystem.OnInteract -= HandleInteraction;
        MapManager.Instance?.UnRegisterPortal(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInPortal = true;
            if (MapManager.Instance != null)
            {
                MapManager.Instance.SetPlayer(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInPortal = false;
        }
    }

    private void HandleInteraction()
    {
        // 영역 안에 있고, F키를 눌렀을 때만 작동
        if (_isPlayerInPortal)
        {
            MapManager.Instance?.WarpInVillage(_targetPortalID);
        }
    }

}
