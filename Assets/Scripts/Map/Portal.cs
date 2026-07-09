using UnityEngine;
using UnityEngine.InputSystem;

public class Portal : MonoBehaviour
{
    [SerializeField] private string _nextMapName;
    [SerializeField] private Transform _spawnPoint;

    private bool _isPlayerInCollider;

    private void OnEnable()
    {
        PlayerInputSystem.OnInteract += Handleinteraction;
    }

    private void OnDisable()
    {
        PlayerInputSystem.OnInteract -= Handleinteraction;
    }

    private void Handleinteraction()
    {
        if (_isPlayerInCollider)
        {
            MapManager.Instance.ChangeMap(_nextMapName, _spawnPoint.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (MapManager.Instance != null)
            {
                _isPlayerInCollider = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (MapManager.Instance != null)
            {
                _isPlayerInCollider = false;
            }
        }
    }
}