using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Move Setting")]
    [SerializeField] private float _moveSpeed = 15f;
    [SerializeField] private float _rotationSpeed = 25f;

    [Header("Player Camera")]
    [SerializeField] private Camera _playerCamera;

    [Header("Destination Point")]
    [SerializeField] private Transform _spotPoint;

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

    private bool _isAttackAnimPlaying = false;
    public bool IsAttackingAnimPlaying => _isAttackAnimPlaying;
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
    }

    private void Update()
    {
        bool isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        if (isPointerOverUI == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnClickAttack();
            }

            if (_isAttackAnimPlaying == false)
            {

                if (Input.GetMouseButtonDown(1))
                {
                    SetTargetPosition(true);
                }

                else if (Input.GetMouseButton(1))
                {
                    SetTargetPosition(false);
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
        // 공격 애니메이션이 끝나기 전까지 데미지 안들어가게 하는 로직
        if (_isAttackAnimPlaying) return;

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
    }

    private void SetTargetPosition(bool updateSpotMarker = false)
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

                if (updateSpotMarker && _spotPoint != null)
                {
                    _spotPoint.position = _targetPosition + Vector3.up * 0.05f;
                    _spotPoint.gameObject.SetActive(true);

                    ParticleSystem[] particleSystems = _spotPoint.GetComponentsInChildren<ParticleSystem>(true);

                    foreach (ParticleSystem ps in particleSystems)
                    {
                        ps.gameObject.SetActive(true);
                    }

                    if (particleSystems.Length > 0)
                    {
                        particleSystems[0].Play(true);
                    }

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

    public void SetCurrentHp(int hp)
    {
        if (_playerData == null) return;

        _playerData.Hp = hp;
    }
    // 스킬을 사용할 때, 마우스 위치로 보게 하기 위한 메서드
    public void LookAtMousePosition()
    {
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
                }
            }
        }
    }
    // 스킬 시전중 움직이거나 다른 행동을 막기 위한 메서드
    public void SetAttackAnimPlaying(bool isPlaying)
    {
        _isAttackAnimPlaying = isPlaying;
    }

}