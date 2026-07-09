using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SmartPatrolSpeed", story: "[Agent]가 [SpawnPos]와의 거리를 재서 [Radius]보다 멀면 [RunSpeed], 가까우면 [WalkSpeed]로 이동합니다", category: "Action", id: "smart_patrol_speed")]
public partial class SmartPatrolSpeedAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> SpwanPos;
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<float> RunSpeed;
    [SerializeReference] public BlackboardVariable<float> WalkSpeed;

    protected override Status OnStart()
    {
        if (Agent.Value == null)
        {
            return Status.Failure;
        }

        NavMeshAgent nav = Agent.Value.GetComponent<NavMeshAgent>();
        if (nav != null)
        {
            float distanceToHome = Vector3.Distance(Agent.Value.transform.position, SpwanPos.Value);

            if (distanceToHome > Radius.Value)
            {
                nav.speed = Radius.Value;
            }
            else
            {
                nav.speed = WalkSpeed.Value;
            }
        }

        return Status.Success;
    }
}

