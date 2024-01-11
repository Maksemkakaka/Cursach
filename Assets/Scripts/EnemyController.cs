using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int health = 1;
    public Transform target;
    public float chaseRange = 10f;
    public float attackRange = 2f;
    private NavMeshAgent agent;
    public Animator animator;
    private Rigidbody rb;
    public LayerMask playerLayer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Находим игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }


    private void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= chaseRange)
        {
            agent.SetDestination(target.position);
            animator.SetBool("isRunning", true);

            if (distance <= attackRange)
            {
                Attack();
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void PerformAttack()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
        foreach (Collider player in hitPlayers)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            player.GetComponent<FirstPlayer>().GetHit(direction);
        }
    }


    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            animator.SetTrigger("Die");
            animator.SetBool("isRunning", false);
        }
    }

    private void Die()
    {
        agent.enabled = false;
        Destroy(gameObject, 2f); 
    }
}
