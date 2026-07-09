using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private Transform _playerTransform;

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

    public void ChangeMap(string mapName, Vector3 spawnPosition)
    {
        WarpPlayer(spawnPosition);
        Debug.Log($"맵이 {mapName}으로 변경되었습니다.");
    }

    private void WarpPlayer(Vector3 targetPosition)
    {
        if (_playerTransform != null)
        {
            PlayerController controller = _playerTransform.GetComponent<PlayerController>();

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
}