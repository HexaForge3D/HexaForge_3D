using UnityEngine;
using UnityEngine.InputSystem;

public class Test_PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5.0f;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Keyboard.current.wKey.isPressed) moveDirection.z += 1;
        if (Keyboard.current.sKey.isPressed) moveDirection.z -= 1;
        if (Keyboard.current.aKey.isPressed) moveDirection.x -= 1;
        if (Keyboard.current.dKey.isPressed) moveDirection.x += 1;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            transform.position += moveDirection.normalized * _moveSpeed * Time.deltaTime;
        }
    }
}