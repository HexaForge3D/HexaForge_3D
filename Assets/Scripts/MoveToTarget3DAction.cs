using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move To Target 3D", story: "[Agent]가 [Target]을 향해 이동", category: "Action", id: "move_to_target_3d")]
public partial class MoveToTarget3DAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private NavMeshAgent navAgent;
    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;

        navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        if (navAgent == null) return Status.Failure;

        return Status.Running;
    }
    protected override Status OnUpdate()
    {
        if (Target.Value == null) return Status.Failure;

        float currentDistance = Vector3.Distance(Agent.Value.transform.position, Target.Value.transform.position);
        if (currentDistance > 10f)
        {
            return Status.Failure;
        }

        navAgent.SetDestination(Target.Value.transform.position);

        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            return Status.Success;
        }
        return Status.Running;
    }
}
