using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Watchful Wait", story: "[Duration]초 동안 대기합니다. 단, [Player]가 [SightRange] 안에 오면 즉시 깨어납니다", category: "Action", id: "watchful_wait")]
public partial class WatchfulWaitAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<float> SightRange;
    [SerializeReference] public BlackboardVariable<float> Duration;

    private float timer;

    protected override Status OnStart()
    {
        timer = 0f;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Player.Value != null && Agent.Value != null)
        {
            float dist = Vector3.Distance(Agent.Value.transform.position, Player.Value.transform.position);
            if (dist <= SightRange.Value)
            {
                return Status.Failure;
            }
        }

        timer += Time.deltaTime;
        if (timer >= Duration.Value)
        {
            return Status.Success;
        }

        return Status.Running;
    }
}

