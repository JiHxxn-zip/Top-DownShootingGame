using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;

    [Header("트레일 컬러")]
    [SerializeField] private Color trailColor;

    private float speed = 10;
    private float damage = 1;

    float lifeTime = 3;

    float skinWidth = 0.1f;


    private void Start()
    {
        Destroy(gameObject, lifeTime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 1f, collisionMask);

        // 총알이 생성되었을 때 어떤 충돌체 오브젝트와 이미 겹친 상태일 때
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }

        // 트레일 컬러 Set
        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }


    void Update()
    {
        float moveDistiance = speed * Time.deltaTime;
        CheckCollisions(moveDistiance);
        transform.Translate(Vector3.forward * moveDistiance);
    }

    private void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }   

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }

        Destroy(gameObject);
    }
}
