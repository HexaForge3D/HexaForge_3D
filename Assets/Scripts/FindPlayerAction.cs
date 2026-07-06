using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Player", story: "[Player]를 찾아 [FoundObject]에 저장", category: "Action", id: "find_player")]
public partial class FindPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> FoundObject;

    protected override Status OnStart()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null)
        {
            FoundObject.Value = target;
            return Status.Success;
        }
        return Status.Failure;
    }
}
