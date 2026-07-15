using UnityEngine;
using UnityEngine.AI;

public class AttackFreeze : StateMachineBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        NavMeshAgent nav = animator.GetComponent<NavMeshAgent>();
        if (nav != null && nav.isOnNavMesh)
        {
            nav.ResetPath();
            nav.velocity = Vector3.zero;
        }
    }
}
