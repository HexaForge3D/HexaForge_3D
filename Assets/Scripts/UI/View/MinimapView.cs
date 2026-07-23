using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MinimapView : BaseOverLayUI
{
    [SerializeField] private Button Button_Store;
    [SerializeField] private Button Button_Smithy;
    [SerializeField] private Button Button_Field;

    public static Action<Vector3> OnMoveToPortalRequested;

    private GameObject _currentPlayer;
    private NavMeshAgent _navMeshAgent;

    protected override void Awake()
    {
        base.Awake ();

        if (Button_Store != null)
        {
            Button_Store.onClick.AddListener(OnClickGoToStore);
        }

        if (Button_Smithy != null)
        {
            Button_Smithy.onClick.AddListener(OnClickGoToSmithy);
        }

        if (Button_Field != null)
        {
            Button_Field.onClick.AddListener(OnClickGoToField);
        }
    }

    private void OnClickGoToStore()
    {
        RequestMoveToPortal(PortalType.Store);
    }

    private void OnClickGoToSmithy()
    {
        RequestMoveToPortal(PortalType.Smithy);
    }

    private void OnClickGoToField()
    {
        RequestMoveToPortal(PortalType.DungeonStart);
    }

    private Portal GetCurrentPortalInRealMap(PortalType portalType)
    {
        var currentPortal = PortalManager.Instance.GetPortal("Village", portalType);
        return currentPortal;
    }

    private void RequestMoveToPortal(PortalType portalType)
    {
        var currentPortal = GetCurrentPortalInRealMap(portalType);

        if (currentPortal != null)
        {
            UIManager.Instance.CloseUI(UIType_This);
            OnMoveToPortalRequested?.Invoke(currentPortal.transform.position);
        }
    }
}
