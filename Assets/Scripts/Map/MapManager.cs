using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class MapSpawnPoint
{
    public string MapName;
    public Transform SpawnPoint;
}

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private List<MapSpawnPoint> _mapSpawnPoints;

    private PlayerController _playerController;

    private Dictionary<string, Transform> _spawnPointMap;

    private Dictionary<string, Portal_Village> _villagePortalGroup;

    private void Awake()
    {
        _villagePortalGroup = new Dictionary<string, Portal_Village>();

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[MapManager:Awake] 현재 인스턴스가 존재하여 중복 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BuildSpawnPointMap();
    }

    private void BuildSpawnPointMap()
    {
        _spawnPointMap = new Dictionary<string, Transform>();

        foreach (MapSpawnPoint entry in _mapSpawnPoints)
        {
            if (_spawnPointMap.ContainsKey(entry.MapName) == false)
            {
                _spawnPointMap.Add(entry.MapName, entry.SpawnPoint);
            }
        }
    }

    public void ChangeMap(string mapName)
    {
        if (_spawnPointMap.TryGetValue(mapName, out Transform spawnPoint) == false)
        {
            Debug.LogError($"[MapManager] {mapName}에 대한 스폰 위치가 등록되어 있지 않습니다.");
            return;
        }

        WarpPlayer(spawnPoint.position);
        Debug.Log($"맵이 {mapName}으로 변경되었습니다.");
    }

    public void WarpInVillage(string targetPortalId)
    {
        if (_villagePortalGroup.TryGetValue(targetPortalId, out var targetPortal) == false) return;
        WarpPlayer(targetPortal.transform.position);
    }

    public void SetPlayer(Transform playerTransform)
    {
        _playerController = playerTransform.GetComponent<PlayerController>();
    }

    private void WarpPlayer(Vector3 targetPosition)
    {
        if (_playerController != null)
        {
            PlayerController controller = _playerController.GetComponent<PlayerController>();

            if (controller != null)
            {
                controller.WarpPosition(targetPosition);
            }

            else
            {
                Debug.LogError("플레이어의 컨트롤러를 가져오지 못했습니다.");
            }
            //_playerTransform.position = targetPosition;
            Debug.Log($"플레이어가 {targetPosition} 위치로 이동했습니다.");
        }
    }

    public void RegisterPortal(Portal_Village portal)
    {
        if (_villagePortalGroup.ContainsKey(portal.PortalID()) == false)
        {
            _villagePortalGroup.Add(portal.PortalID(), portal);
        }
    }

    public void UnRegisterPortal(Portal_Village portal)
    {
        if (_villagePortalGroup.ContainsKey(portal.PortalID()))
        {
            _villagePortalGroup.Remove(portal.PortalID());
        }
    }

    public Transform TargetPotalTransform(string targetPortalId)
    {
        if (_villagePortalGroup.TryGetValue(targetPortalId, out var targetPortal))
        {
            return targetPortal.transform;
        }

        return null;
    }
}