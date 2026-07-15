using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Random Attack", story: "[Agent]가 [Target]을 향해 조준한 뒤 [MaxAttack]가지 중 랜덤 공격을 합니다", category: "Action", id: "random_attack")]
public partial class RandomAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [SerializeReference] public BlackboardVariable<int> MaxAttack;

    private float rotationSpeed = 10f;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;

        UnityEngine.AI.NavMeshAgent nav = Agent.Value.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (nav != null && nav.isOnNavMesh)
        {
            nav.ResetPath();
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;

        Vector3 direction = (Target.Value.transform.position - Agent.Value.transform.position);
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            Agent.Value.transform.rotation = Quaternion.Slerp(Agent.Value.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            float angleDiff = Quaternion.Angle(Agent.Value.transform.rotation, targetRotation);

            if (angleDiff <= 45f)
            {
                ExecuteRandomAttack();
                return Status.Success;
            }
        }
        else
        {
            ExecuteRandomAttack();
            return Status.Success;
        }

        return Status.Running;
    }

    private void ExecuteRandomAttack()
    {
        Animator anim = Agent.Value.GetComponent<Animator>();
        if (anim != null)
        {
            int randomNum = UnityEngine.Random.Range(0, MaxAttack.Value);
            anim.SetInteger("AttackType", randomNum);
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Attack");
        }
    }
}