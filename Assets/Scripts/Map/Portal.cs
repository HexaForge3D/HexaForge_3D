using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private string _nextMapName;
    [SerializeField] private Transform _spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (MapManager.Instance != null)
            {
                MapManager.Instance.ChangeMap(_nextMapName, _spawnPoint.position);
            }
            else
            {
                Debug.LogError("MapManager.Instance가 null입니다. 씬에 MapManager가 배치되어 있는지 확인하세요.");
            }
        }
    }
}