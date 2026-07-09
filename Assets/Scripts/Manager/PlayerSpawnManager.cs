using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerSpawnManager : BaseMonoManager<PlayerSpawnManager>
{
    [SerializeField] private string PlayerPrefabAddress = "Player_01";
    [SerializeField] private Transform SpawnPoint;

    private GameObject _currentPlayer;

    public async UniTask<GameObject> SpawnPlayerAsync(PlayerData data)
    {

        DeSpawnPlayer();

        GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(PlayerPrefabAddress);

        if (prefab == null)
        {
            Debug.LogError($"[PlayerSpawnManager] {PlayerPrefabAddress} 주소에 프리팹이 없습니다.");
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
        Debug.Log($"[PlayerSpawnManager] 플레이어 생성 및 초기화: {data.Name} ({data.Job}) HP:{data.Hp} MP:{data.Mp} ATK:{data.Atk} DEF:{data.Def}");
    }
}
