using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandomPatrolAction", story: "[Center]를 중심으로 [Radius] 반경 내의 랜덤 위치를 찾아 [ResultPosition]에 저장합니다", category: "Action", id: "random_patrol")]
public partial class RandomPatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<Vector3> Center;
    [SerializeReference] public BlackboardVariable<Vector3> ResultPosition;

    protected override Status OnStart()
    {
        if (Agent.Value == null)
        {
            return Status.Failure;
        }
        if (Center.Value == Vector3.zero)
        {
            Center.Value = Agent.Value.transform.position;
        }

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * Radius.Value;
        randomDirection += Center.Value;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, Radius.Value, NavMesh.AllAreas))
        {
            ResultPosition.Value = hit.position;
            return Status.Success;
        }

        ResultPosition.Value = Center.Value;
        return Status.Success;
    }
}

