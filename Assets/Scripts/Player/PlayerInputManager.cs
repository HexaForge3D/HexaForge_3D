using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private KeyCode _playerStop = KeyCode.S;
    
    private PlayerController _playerController;
    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_playerStop))
        {
            _playerController.MoveStop();
        }
    }
}