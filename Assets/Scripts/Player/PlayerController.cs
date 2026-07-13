using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("Move Setting")]
    [SerializeField] private float _moveSpeed = 15f;
    [SerializeField] private float _rotationSpeed = 25f;

    [Header("Player Camera")]
    [SerializeField] private Camera _playerCamera;

    [Header("Destination Point")]
    [SerializeField] private Transform _spotPoint;
    [SerializeField] private float _disappearTime = 0.3f;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask _clickableLayer;

    [Header("Animator")]
    [SerializeField] private Animator _animator;

    private CharacterSaveData _playerData;
    public CharacterSaveData PlayerData => _playerData;

    private Vector3 _targetPosition;
    private Vector3 _lastSetDestination = Vector3.zero;
    private bool _isMoving = false;

    private NavMeshAgent _agent;
    private Rigidbody _rb;
    public float MoveSpeed => _moveSpeed;
    private float _spotTimer = 0f;

    private bool _isAttackAnimPlaying = false;
    private bool _isAttacking = false;
    private Quaternion _attackTargetRotation;
    private PlayerBattle _playerBattle;

    private void Start()
    {
        _targetPosition = transform.position;
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _playerBattle = GetComponent<PlayerBattle>();

        if (_spotPoint != null)
        {
            _spotPoint.gameObject.SetActive(false);
        }

        if (_rb != null)
        {
            _rb.isKinematic = true;
        }

        if (_agent != null)
        {
            _agent.speed = _moveSpeed;
            _agent.updateRotation = false;
        }

       //if (GameDataManager.Instance != null)
       //{
       //    _playerData = GameDataManager.Instance.GetData<PlayerTableData>("Player_01");
         
       //    if (_playerData == null)
       //    {
       //        Debug.LogError("플레이어의 데이터를 찾지 못했습니다.");
       //    }
       //}

       //else
       //{
       //    Debug.LogWarning("GameDataManager가 Scene에 없습니다.");
       //}
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            OnClickAttack();
        }
        if (!_isAttackAnimPlaying)
        {
            if (Input.GetMouseButtonDown(1))
            {
                _spotTimer = 0f;
                if (_spotPoint != null) _spotPoint.gameObject.SetActive(true);
                SetTargetPosition();
            }

            else if (Input.GetMouseButton(1))
            {
                SetTargetPosition();

                if (_spotPoint != null && _spotPoint.gameObject.activeSelf)
                {
                    _spotTimer += Time.deltaTime;
                    if (_spotTimer >= _disappearTime)
                    {
                        _spotPoint.gameObject.SetActive(false);
                    }
                }
            }

            else if (Input.GetMouseButtonUp(1))
            {
                if (_spotPoint != null)
                {
                    _spotPoint.gameObject.SetActive(true);
                }
            }
        }
            if (_agent != null && _agent.speed != _moveSpeed)
            {
                _agent.speed = _moveSpeed;
            }

            if (_isAttacking)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _attackTargetRotation, _rotationSpeed * Time.deltaTime);

                if (Quaternion.Angle(transform.rotation, _attackTargetRotation) < 0.5f)
                {
                    transform.rotation = _attackTargetRotation;
                    _isAttacking = false;
                }
            }

            if (_isMoving)
            {
                MovePlayer();
            }

        _animator.SetBool("isWalking", _isMoving);
    }

    public void InitializePlayerData(CharacterSaveData playerData)
    {
        _playerData = playerData;
    }

    public void OnClickAttack()
    {
        _isAttackAnimPlaying = true;

        if (_playerCamera != null)
        {
            Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
           
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _clickableLayer))
            {
                Vector3 lookDirection = hit.point - transform.position;
                lookDirection.y = 0f;

                if (lookDirection.sqrMagnitude > 0.01f)
                {
                    _attackTargetRotation = Quaternion.LookRotation(lookDirection.normalized);

                    if (_agent != null)
                    {
                        _agent.isStopped = true;
                        _agent.ResetPath();
                    }


                    _isMoving = false;

                    if (_spotPoint != null) _spotPoint.gameObject.SetActive(false);

                    _animator.SetBool("isWalking", _isMoving);

                    _isAttacking = true;

                    FireAnimationTrigger("isAttack");
                }
            }
        }

        if (_playerBattle != null)
        {
            _playerBattle.ExecuteAttack();
        }
    }

    private void SetTargetPosition()
    {
        if (_playerCamera == null) return;

        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _clickableLayer))
        {
            if (Vector3.Distance(_lastSetDestination, hit.point) < 0.2f) return;
            _lastSetDestination = hit.point;

            Vector3 rawTargetPosition = hit.point;

            if (Vector3.Dot(hit.normal, Vector3.up) < 0.9f)
            {
                if (hit.collider != null && !hit.collider.isTrigger)
                {
                    Vector3 boundsCenter = hit.collider.bounds.center;
                    Vector3 dirFromCenter = (hit.point - boundsCenter).normalized;
                    dirFromCenter.y = 0;

                    Vector3 safeEdge = hit.collider.ClosestPointOnBounds(hit.point);
                    rawTargetPosition = safeEdge + dirFromCenter * 1.5f;
                }
            }

            // SamplePosition으로 레이캐스트가 맞은 곳에서 가장 가까운 "실제 네비메쉬 바닥"을 정밀하게 탐색합니다.
            if (NavMesh.SamplePosition(rawTargetPosition, out NavMeshHit navHit, 3.0f, NavMesh.AllAreas))
            {
                _targetPosition = navHit.position;

                if (_agent != null)
                {
                    _isAttacking = false;
                    _agent.SetDestination(_targetPosition);
                    _agent.isStopped = false;
                }

                if (_spotPoint != null)
                {
                    _spotPoint.position = _targetPosition + Vector3.up * 0.05f;
                }
                _isMoving = true;
            }
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
                if (_spotPoint != null) _spotPoint.gameObject.SetActive(false);
            }
        }
        if (_isAttacking == true)
        {
            _isMoving = false;
            if (_spotPoint != null) _spotPoint.gameObject.SetActive(false);
        }
    }

    public void MoveStop()
    {
        _isMoving = false;
        _targetPosition = transform.position;

        if (_spotPoint != null)
        {
            _spotPoint.gameObject.SetActive(false);
        }

        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
    }

    public void WarpPosition(Vector3 targetPosition)
    {
        _isMoving = false;
        if (_spotPoint != null) _spotPoint.gameObject.SetActive(false);

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

    public void FireAnimationTrigger(string animationName)
    {
        _animator.SetTrigger(animationName);
    }

    public void OnAttackAnimEnd()
    {
        _isAttackAnimPlaying = false;
    }
}