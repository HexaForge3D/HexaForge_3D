using UnityEngine;
using UnityEngine.AI;
public class PlayerController : MonoBehaviour
{
    [Header("Move Setting")]
    [SerializeField] private float _moveSpeed = 5f; // 이동속도
    [SerializeField] private float _rotationSpeed = 8f; // 회전속도

    [Header("Player Camera")]
    [SerializeField] private Camera _playerCamera;

    private Vector3 _targetPosition;
    private bool _isMoving = false;

    private NavMeshAgent _agent;
    private Rigidbody _rb;
    public float MoveSpeed => _moveSpeed;

    private void Start()
    {
        _targetPosition = transform.position;
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();

        if (_rb != null)
        {
            _rb.isKinematic = true;
        }

        if (_agent != null)
        {
            _agent.speed = _moveSpeed;
            _agent.updateRotation = false;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 몬스터 공격하는 평타 기능 추가 (07/06에 추가 => 이후에 기능 추가할 것)
        }

        if (Input.GetMouseButtonDown(1))
        {
            SetTargetPosition();
        }

        if (_agent != null)
        {
            _agent.speed = _moveSpeed;
        }

        if (_isMoving)
        {
            MovePlayer();
        }
    }

    private void SetTargetPosition()
    {
        if (_playerCamera == null) return;
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            _targetPosition = hit.point;
            _targetPosition.y = transform.position.y;

            if (_agent != null)
            {
                _agent.SetDestination(_targetPosition);
                _agent.isStopped = false;
            }

            _isMoving = true;
        }
    }

    private void MovePlayer()
    {
        if (_agent != null)
        {
            Vector3 direction = _agent.velocity;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _rotationSpeed * Time.deltaTime);
            }

            if (_agent.pathPending == false && _agent.remainingDistance <= 0.05f)
            {
                _isMoving = false;
            }
        }
    }

    public void MoveStop()
    {
        _isMoving = false;
        _targetPosition = transform.position;
        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
    }
}