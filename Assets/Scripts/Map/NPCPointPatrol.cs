using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;

public class NPCPointPatrol : BaseDungeonController
{
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private float _arrivalThreshold = 1f;
    [SerializeField] private float _detectRadius = 10f;
    [SerializeField] private string _monsterTag = "Monster";
    [SerializeField] private string _playerTag = "Player";

    private NavMeshAgent _navMeshAgent;
    private int _currentIndex = 0;
    private bool _isFinished = false;
    private bool _isPlayerInCollider = false;
    private bool _isStarted = false;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_navMeshAgent != null)
        {
            _navMeshAgent.isStopped = true;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        PlayerInputSystem.OnInteract += HandleInteraction;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PlayerInputSystem.OnInteract -= HandleInteraction;
    }

    private void Update()
    {
        if (!_isStarted || _isFinished)
        {
            return;
        }

        bool hasMonster = HasTargetInDetectRadius(_monsterTag);
        bool hasPlayer = HasTargetInDetectRadius(_playerTag);

        if (hasMonster || !hasPlayer)
        {
            if (_navMeshAgent != null && !_navMeshAgent.isStopped)
            {
                _navMeshAgent.isStopped = true;
            }
            return;
        }

        if (_navMeshAgent != null && _navMeshAgent.isStopped)
        {
            _navMeshAgent.isStopped = false;
            MoveToNextWaypoint();
        }

        CheckWaypoint();
    }

    private void HandleInteraction()
    {
        if (_isPlayerInCollider && !_isStarted)
        {
            StartPatrol();
        }
    }

    private void StartPatrol()
    {
        _isStarted = true;
        if (_navMeshAgent != null)
        {
            _navMeshAgent.isStopped = false;
        }
        MoveToNextWaypoint();
        Debug.Log("플레이어와의 상호작용으로 NPC 패트롤을 시작합니다.");
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

                DungeonReward reward = new DungeonReward
                {
                    Gold = 100,
                    ItemIds = new List<string>()
                };
                
                InvokeCleared(reward);
                return;
            }

            _currentIndex++;
            MoveToNextWaypoint();
        }
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
}