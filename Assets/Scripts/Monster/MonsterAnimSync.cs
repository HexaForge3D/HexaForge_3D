using UnityEngine;
using UnityEngine.AI;

public class MonsterAnimSync : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (agent != null && anim != null)
        {
            anim.SetFloat("MoveSpeed", agent.velocity.magnitude);
        }
    }
}
