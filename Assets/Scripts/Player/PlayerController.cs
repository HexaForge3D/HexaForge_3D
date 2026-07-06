using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move Setting")]
    [SerializeField] private float _moveSpeed = 5f; // 이동속도
    [SerializeField] private float _rotationSpeed = 8f; // 회전속도

    private Vector3 _targetPosition;
    private bool _isMoving = false;

    private void Start()
    {
        _targetPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetTargetPosition();
        }

        if (_isMoving)
        {
            MovePlayer();
        }
    }

    private void SetTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            _targetPosition = hit.point;

            _targetPosition.y = transform.position.y;

            _isMoving = true;
        }
    }

    private void MovePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
        Vector3 direction = (_targetPosition - transform.position).normalized;
        
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, _targetPosition) <= 0.05f)
        {
            _isMoving = false;
        }
    }
}