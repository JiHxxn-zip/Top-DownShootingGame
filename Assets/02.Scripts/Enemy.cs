using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State
    {
        Idle, Chasing, Attackig
    }

    private State currentState;

    private NavMeshAgent pathfinder;

    private Transform target;

    private LivingEntity targetEntity;

    private Material skinMaterial;

    private Color originalColor;


    // 공격할 수 있는 거리 (1.5m)
    private float attackDistanceThreshold = 0.5f;

    // 공격할 수 있는 시간
    private float timeBetweenAttacks = 1;

    // 대미지
    private float damage = 1;

    // 다음 공격 가능 시간
    private float nextAttackTime;

    // 나와 타겟의 반지름(공격시 겹침 방지)
    private float myCollisionRadius;
    private float targetCollisionRadius;

    // 타겟 존재여부
    bool hasTarget;


    protected override void Start()
    {
        base.Start();

        pathfinder = GetComponent<NavMeshAgent>();

        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;

        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
    }

    private void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    private void Update()
    {
        if(hasTarget)
        {
            if (Time.time > nextAttackTime == false)
            {
                return;
            }

            float sqrDisToTarget = (target.position - transform.position).sqrMagnitude;

            if (sqrDisToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
            {
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        currentState = State.Attackig;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;

        Vector3 attackPosion = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.red;

        // 대미지를 적용하는 도중인가
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if(percent >= 0.5f && hasAppliedDamage == false)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosion, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while(hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);

                if (!isDead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }

            yield return new WaitForSeconds(refreshRate);
        }
    }
}
