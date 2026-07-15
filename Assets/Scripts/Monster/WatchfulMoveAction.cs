using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Watchful Move", story: "[Agent]가 [TargetPos]로 걷습니다. 단, [Player]가 [SightRange] 안에 오면 걷기를 즉시 중단합니다", category: "Action", id: "watchful_move")]
public partial class WatchfulMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPos;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<float> SightRange;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;
        Agent.Value.GetComponent<NavMeshAgent>().SetDestination(TargetPos.Value);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null) return Status.Failure;
        UnityEngine.AI.NavMeshAgent nav = Agent.Value.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (Player.Value != null && Vector3.Distance(Agent.Value.transform.position, Player.Value.transform.position) <= 10f)
        {
            nav.ResetPath();
            return Status.Success;
        }

        if (!nav.pathPending)
        {
            if (nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial)
            {
                return Status.Success;
            }

            if (nav.remainingDistance <= nav.stoppingDistance + 0.1f)
            {
                return Status.Success;
            }
        }

        return Status.Running;
    }
}

