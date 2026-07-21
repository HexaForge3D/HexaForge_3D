using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

    private VillageAreaRepository _mapRepository = new VillageAreaRepository();
    private HuntingAreaRepository _huntingMapRepository = new HuntingAreaRepository();
    private GameObject _currentMapInstance;
    private string _currentMapAddress;
    private PlayerController _playerController;

    private PortalType _lastPortalType = PortalType.None;

    [SerializeField] private List<MapSpawnPoint> _mapSpawnPoints;

    private Dictionary<string, Transform> _spawnPointMap;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[MapManager:Awake] 현재 인스턴스가 존재하여 중복 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public async UniTask ChangeMapAsync(string mapId, PortalType entryPortalType = PortalType.Village)
    {
        _lastPortalType = entryPortalType;

        VillageAreaData mapData = _mapRepository.GetArea(mapId);
        HuntingAreaData huntingMapData = _huntingMapRepository.GetArea(mapId);
        if (mapData == null && huntingMapData == null)
        {
            Debug.LogWarning($"[MapManager]{mapId}에 대한 맵데이터가 존재하지 않습니다.");
            return;
        }

        if (ResourceManager.Inst == null)
        {
            Debug.LogError("[MapManager] ResourceManager.Inst (싱글톤)가 씬에 존재하지 않습니다!");
            return;
        }

        string prefabPath = mapData == null ? huntingMapData.PrefabPath : mapData.PrefabPath;

        if (string.IsNullOrEmpty(prefabPath))
        {
            Debug.LogError($"[MapManager] 맵 데이터({mapId})의 PrefabPath가 비어있습니다!");
            return;
        }

        DestroyMap();
        if (mapData != null)
        {
            _currentMapInstance = await ResourceManager.Inst.InstantiateAsync(mapData.PrefabPath);
            _currentMapAddress = mapData.PrefabPath;
            InstantiateVillageMap(mapData);
        }
        else if (huntingMapData != null)
        {
            _currentMapInstance = await ResourceManager.Inst.InstantiateAsync(huntingMapData.PrefabPath);
            _currentMapAddress = huntingMapData.PrefabPath;
            InstantiateHuntingMap(huntingMapData);
        }
    }

    private void DestroyMap()
    {
        if (_currentMapInstance != null)
        {
            Destroy(_currentMapInstance);
            _currentMapInstance = null;

            if (string.IsNullOrEmpty(_currentMapAddress) == false)
            {
                ResourceManager.Inst.Release(_currentMapAddress);
            }
        }
    }

    private void InstantiateVillageMap(VillageAreaData mapData)
    {
        if (_currentMapInstance == null)
        {
            Debug.LogError($"[MapManager] 맵 생성 실패: {mapData.PrefabPath}");
            return;
        }

        WarpPlayerToCurrentPortal(mapData);

        Debug.Log($"맵이 {mapData.Name}(으)로 변경되었습니다.");
    }

    private void InstantiateHuntingMap(HuntingAreaData mapData)
    {
        if (_currentMapInstance == null)
        {
            Debug.LogError($"[MapManager] 맵 생성 실패: {mapData.PrefabPath}");
            return;
        }

        Portal targetPortal = PortalManager.Instance.GetPortal(mapData.MapName, PortalType.DungeonStart);

        if (targetPortal != null)
        {
            WarpPlayer(targetPortal.transform.position);
        }
        else
        {
            Debug.LogWarning($"[MapManager] {mapData.MapName}에서 스폰할 포탈을 찾지 못했습니다.");
        }

        Debug.Log($"맵이 {mapData.Name}(으)로 변경되었습니다.");
    }

    private void WarpPlayerToCurrentPortal(VillageAreaData mapData)
    {
        PortalType portalTypeToFind = mapData.SpawnPortalType;

        if (mapData.MapName == "Village")
        {
            portalTypeToFind = _lastPortalType;
        }

        Portal targetPortal = PortalManager.Instance.GetPortal(mapData.MapName, portalTypeToFind);

        if (targetPortal != null)
        {
            WarpPlayer(targetPortal.transform.position);
        }
        else
        {
            Debug.LogWarning($"[MapManager] {mapData.MapName}에서 스폰할 포탈을 찾지 못했습니다.");
        }
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
                return;
            }
            Debug.Log($"플레이어가 {targetPosition} 위치로 이동했습니다.");
        }
    }
}