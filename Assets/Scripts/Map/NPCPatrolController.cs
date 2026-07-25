using UnityEngine;
using UnityEngine.AI;
using System;

public class NPCPatrolController : MonoBehaviour
{
    [Header("NPC경로오브젝트")]
    [SerializeField] private Transform[] _waypoints;

    [SerializeField] private float _arrivalThreshold = 1f;
    [SerializeField] private float _detectRadius = 10f;
    [SerializeField] private string _monsterTag = "Monster";
    [SerializeField] private string _playerTag = "Player";

    public static event Action OnPatrolFinished;
    public static event Action<int, int> OnWaypointChanged;

    private NavMeshAgent _navMeshAgent;
    private int _currentIndex = 0;
    private bool _isFinished = false;
    private bool _isPlayerInCollider = false;
    private bool _isStarted = false;

    private Animator _animator;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        if (_navMeshAgent != null)
        {
            _navMeshAgent.isStopped = true;
        }
    }

    private void OnEnable()
    {
        PlayerInputSystem.OnInteract += HandleInteraction;
    }

    private void OnDisable()
    {
        PlayerInputSystem.OnInteract -= HandleInteraction;
    }

    private void Update()
    {
        if ((_isStarted == false) || _isFinished)
        {
            SetAnimation(false, false);
            return;
        }

        bool hasMonster = HasTargetInDetectRadius(_monsterTag);
        bool hasPlayer = HasTargetInDetectRadius(_playerTag);

        if (hasMonster || !hasPlayer)
        {
            if (_navMeshAgent != null && (_navMeshAgent.isStopped == false))
            {
                _navMeshAgent.isStopped = true;
            }
            SetAnimation(false, true);
            return;
        }

        if (_navMeshAgent != null && _navMeshAgent.isStopped)
        {
            _navMeshAgent.isStopped = false;
            MoveToNextWaypoint();
        }

        SetAnimation(true, false);
        CheckWaypoint();
    }

    private void HandleInteraction()
    {
        if (_isPlayerInCollider && (_isStarted == false))
        {
            StartPatrol();
        }
    }

    private void StartPatrol()
    {
        if (_isStarted)
        {
            return;
        }

        _isStarted = true;
        if (_navMeshAgent != null)
        {
            _navMeshAgent.isStopped = false;
        }
        MoveToNextWaypoint();
        HandleWaypointChanged();

        Debug.Log("플레이어 상호작용! NPC 패트롤을 시작합니다.");
    }

    private bool HasTargetInDetectRadius(string targetTag)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject && hitCollider.CompareTag(targetTag))
            {
                return true;
            }
        }

        return false;
    }

    private void MoveToNextWaypoint()
    {
        if (_waypoints == null || _waypoints.Length == 0 || _isFinished)
        {
            return;
        }

        _navMeshAgent.SetDestination(_waypoints[_currentIndex].position);
    }

    private void CheckWaypoint()
    {
        if (_waypoints == null || _waypoints.Length == 0 || _isFinished)
        {
            return;
        }

        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _arrivalThreshold)
        {
            if (_currentIndex >= _waypoints.Length - 1)
            {
                _isFinished = true;
                _navMeshAgent.isStopped = true;
                Debug.Log("모든 웨이포인트 경로를 완료했습니다.");

                SetAnimation(false, false);

                OnPatrolFinished?.Invoke();
                return;
            }

            _currentIndex++;
            MoveToNextWaypoint();
            HandleWaypointChanged();
        }
    }

    private void HandleWaypointChanged()
    {
        if (_waypoints == null || _waypoints.Length == 0)
        { 
            return; 
        }

        OnWaypointChanged?.Invoke(_currentIndex + 1, _waypoints.Length);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInCollider = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectRadius);
    }

    private void SetAnimation(bool isWalk, bool isScary)
    {
        if (_animator != null)
        {
            _animator.SetBool("IsWalk", isWalk);
            _animator.SetBool("IsScary", isScary);
        }
    }
}