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

    [Header("Animator")]
    [SerializeField] private Animator _anmator;

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
            OnClickAttack();
        }

        if (Input.GetMouseButton(1))
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
        PlayerTableData playerData = GameDataManager.Instance.GetData<PlayerTableData>("Player_01");
        if (playerData != null)
        {
            int atk = playerData.Atk;
            Debug.Log($"{atk}의 데미지로 공격하였습니다.");
        }
        //_anmator.SetTrigger("Attack");
    }


    private void SetTargetPosition()
    {
        if (_playerCamera == null) return;

        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            _targetPosition = hit.point;

            // 2. 바닥이 아닌 장애물(벽, 기둥 등)을 클릭했는지 판별
            // hit.normal(표면이 바라보는 방향)이 위쪽(Vector3.up)이 아니면 장애물로 간주합니다.
            if (Vector3.Dot(hit.normal, Vector3.up) < 0.9f)
            {
                if (hit.collider != null && !hit.collider.isTrigger)
                {
                    // [핵심] 질문자님 아이디어: 중심을 찾아서 바깥으로 밀어내기
                    Vector3 boundsCenter = hit.collider.bounds.center;

                    // 중심에서 마우스 클릭 지점으로 향하는 방향 벡터 계산 (Y축 높낮이는 무시)
                    Vector3 dirFromCenter = (hit.point - boundsCenter).normalized;
                    dirFromCenter.y = 0;

                    // 에러가 나는 ClosestPoint 대신 모든 콜라이더에 안전한 ClosestPointOnBounds 사용
                    Vector3 safeEdge = hit.collider.ClosestPointOnBounds(hit.point);

                    // 콜라이더 외곽에서 바깥쪽으로 살짝 밀어냅니다.
                    _targetPosition = safeEdge + dirFromCenter * 1.5f;
                }
            }

            _targetPosition.y = transform.position.y;

            if (NavMesh.SamplePosition(_targetPosition, out NavMeshHit navHit, 1.5f, NavMesh.AllAreas))
            {
                _targetPosition = navHit.position;
            }

            if (_agent != null)
            {
                _agent.SetDestination(_targetPosition);
                _agent.isStopped = false;
            }

            _spotPoint.gameObject.SetActive(true);
            _spotPoint.position = _targetPosition;
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

    public void WarpPosition(Vector3 targetPosition)
    {
        _isMoving = false;
        _spotPoint.gameObject.SetActive(false);

        if (_agent != null && _agent.isActiveAndEnabled)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
            _agent.Warp(targetPosition);
        }

        else
        {
            transform.position = targetPosition;
        }
    }
}