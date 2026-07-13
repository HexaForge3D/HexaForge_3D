using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move To Position", story: "[Agent]가 [TargetPosition]으로 이동", category: "Action", id: "move_to_position")]
public partial class MoveToPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

    private NavMeshAgent navAgent;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;
        navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        if (navAgent == null) return Status.Failure;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        navAgent.SetDestination(TargetPosition.Value);

        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            return Status.Success;
        }
        return Status.Running;
    }
}

