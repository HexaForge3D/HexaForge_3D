using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Monster Attack", story: "[Agent]가 [Target]을 향해 공격 애니메이션을 재생합니다", category: "Action", id: "monster_attack")]
public partial class MonsterAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private float rotationSpeed = 10f;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        Collider targetCollider = Target.Value.GetComponent<Collider>();
        if (targetCollider == null || targetCollider.enabled == false)
        {
            Target.Value = null;
            return Status.Failure;
        }

        UnityEngine.AI.NavMeshAgent nav = Agent.Value.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (nav != null && nav.isOnNavMesh) nav.ResetPath();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        Vector3 direction = (Target.Value.transform.position - Agent.Value.transform.position);
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

            Agent.Value.transform.rotation = Quaternion.Slerp(Agent.Value.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            float angleDiff = Quaternion.Angle(Agent.Value.transform.rotation, targetRotation);

            if (angleDiff <= 10f)
            {
                Animator anim = Agent.Value.GetComponent<Animator>();
                if (anim != null) anim.SetTrigger("Attack");

                return Status.Success;
            }
        }
        else
        {
            Animator anim = Agent.Value.GetComponent<Animator>();
            if (anim != null) anim.SetTrigger("Attack");
            return Status.Success;
        }

        return Status.Running;
    }
}