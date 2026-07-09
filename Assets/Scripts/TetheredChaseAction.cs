using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Tethered Chase Player", story: "[Agent]가 [Target]을 쫓습니다. (단, [SpawnPos]에서 [LeashRange]를 벗어나면 포기하고 복귀)", category: "Action", id: "tethered_chase_player")]
public partial class TetheredChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<Vector3> SpawnPos;
    [SerializeReference] public BlackboardVariable<float> LeashRange;
    [SerializeReference] public BlackboardVariable<float> AttackRange;

    private bool isGivingUp = false;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;

        isGivingUp = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;

        NavMeshAgent nav = Agent.Value.GetComponent<NavMeshAgent>();
        if (nav == null) return Status.Failure;

        if (isGivingUp)
        {
            nav.SetDestination(SpawnPos.Value);

            if (Vector3.Distance(Agent.Value.transform.position, SpawnPos.Value) <= 1.5f)
            {
                isGivingUp = false;
                return Status.Failure;
            }

            return Status.Running;
        }

        float distanceFromHome = Vector3.Distance(Agent.Value.transform.position, SpawnPos.Value);

        if (distanceFromHome > LeashRange.Value)
        {
            isGivingUp = true;
            return Status.Running;
        }

        float distanceToPlayer = Vector3.Distance(Agent.Value.transform.position, Target.Value.transform.position);
        if (distanceToPlayer <= AttackRange.Value)
        {
            nav.ResetPath();
            nav.velocity = Vector3.zero;

            return Status.Success;
        }

        nav.SetDestination(Target.Value.transform.position);
        return Status.Running;
    }
}