using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    private Animator animator;
    private Transform player;
    public float distance = 3f;
    private bool stood = false;
    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Invoke("standanim", 8.267f);
    }

    // Update is called once per frame
    void Update()
    {
        var navAgent = GetComponent<NavMeshAgent>();
        animator.SetFloat("speed", navAgent.velocity.magnitude);
    }
 
    private void OnTriggerStay(Collider other)
    { 
        transform.LookAt(player);

        if(stood)
        {
            var navAgent = GetComponent<NavMeshAgent>();

            if (Vector3.Distance(gameObject.transform.position, other.transform.position) < distance)
            {
                navAgent.isStopped = true;
                if (!animator.GetBool("attack"))
                    animator.SetBool("attack", true);
            }
            else
            {
                animator.SetBool("attack", false);
                navAgent.SetDestination(other.transform.position);
                navAgent.isStopped = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var navAgent = GetComponent<NavMeshAgent>();
        navAgent.isStopped = true;
    }

    void standanim()
    {
        stood = true;
    }

    void punchanim()
    {
        animator.SetBool("attack",true);
    }
}
