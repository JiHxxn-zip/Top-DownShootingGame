using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;

    private float speed = 10;
    private float damage = 1;

    float lifeTime = 3;

    float skinWidth = 0.1f;


    private void Start()
    {
        Destroy(gameObject, lifeTime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 1f, collisionMask);

        // �Ѿ��� �����Ǿ��� �� � �浹ü ������Ʈ�� �̹� ��ģ ������ ��
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0]);
        }
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

        if(Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent <IDamageable>();
        if(damageableObject != null)
        {
            damageableObject.TakeHit(damage, hit); 
        }

        Destroy(gameObject);
    }

    void OnHitObject(Collider c)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
