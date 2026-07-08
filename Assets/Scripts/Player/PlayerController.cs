using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
public class PlayerController : MonoBehaviour
{
    [Header("Move Setting")]
    [SerializeField] private float _moveSpeed = 5f; // 이동속도
    [SerializeField] private float _rotationSpeed = 10f; // 회전속도

    [Header("Player Camera")]
    [SerializeField] private Camera _playerCamera;

    [Header("NavMesh Surface")]
    [SerializeField] private Transform _navMeshSurface;
    [SerializeField] private Transform _spotPoint;
    
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
        _navMeshSurface.GetComponent<NavMeshSurface>().BuildNavMesh();
        _spotPoint.gameObject.SetActive(false);

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
            // OnClickAttack();
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
        CheckDistance();
    }

    public void OnClickAttack() // 좌클릭시 공격하는 로직 (메서드 이름은 변경해도 됨)
    {

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
          
            _spotPoint.gameObject.SetActive(true);
            _spotPoint.position = hit.point;
            _isMoving = true;
        }
    }

    private void CheckDistance()
    {
        if (Vector3.Distance(this.transform.position, _navMeshSurface.position) >= 15f)
        {
            _navMeshSurface.transform.position = this.transform.position;
            _navMeshSurface.GetComponent<NavMeshSurface>().BuildNavMesh();
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
                _spotPoint.gameObject.SetActive(false);
            }
        }
    }

    public void MoveStop()
    {
        _isMoving = false;
        _targetPosition = transform.position;
        _spotPoint.gameObject.SetActive(false);
        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
    }
}