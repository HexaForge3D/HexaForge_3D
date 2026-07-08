//using UnityEngine;
//using UnityEngine.InputSystem;

//public class Portal : MonoBehaviour
//{
//    [SerializeField] private string _nextMapName;
//    [SerializeField] private Transform _spawnPoint;

//    private bool _isPlayerInCollider;

//    private void OnEnable()
//    {
//        PlayerInputManager.Oninteract += Handleinteraction;
//    }

//    private void OnDisable()
//    {
//        PlayerInput.OnInteract -= HandleInteraction;
//    }

//    private void Handleinteraction()
//    {
//        if (_isPlayerInCollider)
//        {
//            MapManager.Instance.ChangeMap(_nextMapName, _spawnPoint.position);
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            if (MapManager.Instance != null)
//            {
//                _isPlayerInCollider = true;
//            }
//        }
//    }

//    //private void OnTriggerExit(Collider other)
//    //{
//    //    if (other.CompareTag("Player"))
//    //    {
//    //        if (MapManager.Instance != null)
//    //        {
//    //            _isPlayerInCollider = false;
//    //        }
//    //    }
//    //}
//}