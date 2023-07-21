using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity
{
    private NavMeshAgent pathfinder;

    private Transform target;

    protected override void Start()
    {
        base.Start();

        pathfinder = GetComponent<NavMeshAgent>();

        target = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while(target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);

            if (!isDead)
            {
                pathfinder.SetDestination(targetPosition);
            }

            yield return new WaitForSeconds(refreshRate);
        }
    }
}
