using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Spawn Position", story: "[Agent]의 현재위치를 [SpwanPos]에 저장 ", category: "Action", id: "set_spawn_position")]
public partial class SetSpawnPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> SpawnPos;
    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

        SpawnPos.Value = Agent.Value.transform.position;
        return Status.Success;
    }
}

