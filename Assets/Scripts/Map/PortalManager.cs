using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance;

    private Dictionary<PortalType, Portal> _destinationPortals = new Dictionary<PortalType, Portal>();
    private List <Portal> _allPortals = new List<Portal>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterPortal(Portal portal)
    {
        if (_destinationPortals.TryGetValue(portal.PortalType, out Portal targetPortal))
        {
            targetPortal._partnerPortal = portal;
            portal._partnerPortal = targetPortal;

            _destinationPortals.Remove(portal.PortalType);
        }
        else
        {
            {
                _destinationPortals[portal.PortalType] = portal;
            }
        }
    }

    public void UnRegisterPortal(Portal portal)
    {
        if (portal._partnerPortal != null)
        {
            portal._partnerPortal._partnerPortal = null;
        }
        _destinationPortals.Remove(portal.PortalType);

    }

    public Portal GetDestinationPortal(Portal currentPortal)
    {
        return currentPortal._partnerPortal;
    }

    public Portal GetPortalByType(PortalType type)
    {
        foreach (var portal in  _allPortals)
        {
            if (portal.PortalType == type)
                {
                    return portal;
                }
        }
        return null;
    }
}
