using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

public class PlayerSpawnManager : BaseMonoManager<PlayerSpawnManager>
{
    [SerializeField] private Transform SpawnPoint;

    private GameObject _currentPlayer;

    public async UniTask<GameObject> SpawnPlayerAsync(PlayerData data, string prefabAddress)
    {
        DeSpawnPlayer();

        GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(prefabAddress).ToUniTask();

        if (prefab == null)
        {
            Debug.LogError($"[PlayerSpawnManager] {prefabAddress} 주소에 프리팹이 없습니다.");
            return null;
        }

        Vector3 position = SpawnPoint != null ? SpawnPoint.position : Vector3.zero;
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);

        InitializePlayer(instance, data);

        _currentPlayer = instance;

        return instance;
    }

    public void DeSpawnPlayer()
    {
        if (_currentPlayer != null)
        {
            Destroy(_currentPlayer);
            _currentPlayer = null;
        }
    }

    private void InitializePlayer(GameObject instance, PlayerData data)
    {
        PlayerController controller = instance.GetComponentInChildren<PlayerController>();

        if (controller == null)
        {
            Debug.LogError("[PlayerSpawnManager] 생성된 프리팹에 PlayerController가 없습니다.");
            return;
        }

        CharacterSaveData saveData = SaveManager.Instance.GetChararcterData(data.Id);

        if (saveData == null)
        {
            Debug.LogError($"[PlayerSpawnManager] {data.Id}에 대한 세이브 데이터를 찾을 수 없습니다.");
            return;
        }

        controller.InitializePlayerData(saveData);

        Debug.Log($"[PlayerSpawnManager] 플레이어 생성 및 초기화: {saveData.Name} ({saveData.Job})");
    }

    // 죽고 나면 다시 스폰된 위치로 이동할 수 있는 메서드 추가
    public void MoveToSpawnPoint(GameObject player)
    {
        if (player == null) return;

        Vector3 targetPosition = SpawnPoint != null ? SpawnPoint.position : Vector3.zero;

        NavMeshAgent agent = player.GetComponent<NavMeshAgent>();

        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.Warp(targetPosition);
        }
        
        else
        {
            player.transform.position = targetPosition;
        }
    }

    //현재 스폰된 플레이어의 PlayerBattle를 조회하는 메서드
    public PlayerBattle GetPlayerBattle()
    {
        if (_currentPlayer == null) return null;

        return _currentPlayer.GetComponentInChildren<PlayerBattle>();
    }
}
