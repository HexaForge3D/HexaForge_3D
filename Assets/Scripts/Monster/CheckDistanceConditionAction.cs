using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check Distance", story: "[Agent]와 [Target]의 거리가 [MaxDistance] 이내인지 확인", category: "Condition", id: "check_distance")]
public partial class CheckDistanceCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> MaxDistance;

    public override bool IsTrue()
    {
        if (Agent.Value == null || Target.Value == null) return false;

        float distance = Vector3.Distance(Agent.Value.transform.position, Target.Value.transform.position);
        return distance <= MaxDistance.Value;
    }
}