using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Player Auto", story: "씬에서 [Player] 태그를 찾아 [Target]에 저장합니다", category: "Action", id: "find_player_auto")]
public partial class FindPlayerAutoAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        return Status.Running;
    }
    protected override Status OnUpdate()
    {
        if (Target.Value != null) return Status.Success;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            Target.Value = playerObj;
            return Status.Success;
        }

        return Status.Running;
    }
}

