using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance;

    private Dictionary<PortalType, Portal> _destinationPortals = new Dictionary<PortalType, Portal>();
    private List <Portal> _allPortals = new List<Portal>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterPortal(Portal portal)
    {
        if (_allPortals.Contains(portal) == false)
        {
            _allPortals.Add(portal);
        }

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
        if (_allPortals.Contains(portal) == true)
        {
            _allPortals.Remove(portal);
        }
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

    public Portal GetPortal(string mapName, PortalType portalType)
    {
        foreach (var portal in _allPortals)
        {
            if (portal.ParentMapName == mapName && portal.PortalType == portalType)
            {
                return portal;
            }
        }
        Debug.LogWarning($"[PortalManager] 맵 [{mapName}]에서 타입 [{portalType}]인 포탈을 찾지 못했습니다.");
        return null;
    }
}
