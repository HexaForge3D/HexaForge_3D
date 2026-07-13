using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MonsterSpeedAction", story: "[Agent]의 이동 속도를 [Speed]로 변경합니다", category: "Action", id: "monster_speed")]
public partial class MonsterSpeedAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Speed;

    protected override Status OnStart()
    {
        if (Agent.Value == null)
        {
            return Status.Failure;
        }

        NavMeshAgent nav = Agent.Value.GetComponent<NavMeshAgent>();
        if (nav != null)
        {
            nav.speed = Speed.Value;
        }

        return Status.Success;
    }
}

