using Cysharp.Threading.Tasks;
using UnityEngine;

public enum PortalType : byte
{
    None,
    Village,
    DungeonStart,
    DungeonClear,
    Store,
    MainQuest,
    Smithy,
    FakePortal
}

public class Portal : InteractObject
{
    public static event System.Action<Portal> OnPortalInteracted;

    [SerializeField] private PortalType _portalType;
    public PortalType PortalType => _portalType;

    [Header("이동할 맵 설정")]
    [SerializeField] private string _targetMapId;
    public string TargetMapId => _targetMapId;

    [Header("포탈이 속한 맵 이름")]
    [SerializeField] private string _parentMapName;
    public string ParentMapName => _parentMapName;

    public Portal _partnerPortal { get; set; }

    // 인터페이스 멤버 =======================
    protected override void OnEnable()
    {
        PortalManager.Instance.RegisterPortal(this);
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        PortalManager.Instance.UnRegisterPortal(this);
        base.OnDisable();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _isPlayerInCollider = true;
            MapManager.Instance.SetPlayer(other.transform);
            EnterPortal();
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _isPlayerInCollider = false;
            if (MapManager.Instance != null)
            {
                ExitPortal();
            }
        }
    }

    protected override bool CanInteract()
    {
        return true;
    }

    protected override void OnInteract()
    {
        OnPortalInteracted?.Invoke(this);
    }
    public void EnterPortal()
    {
        _isPlayerInCollider = true;
        
    }
    public void ExitPortal()
    {
        if (MapManager.Instance != null)
        {
            _isPlayerInCollider = false;
        }
    }
}