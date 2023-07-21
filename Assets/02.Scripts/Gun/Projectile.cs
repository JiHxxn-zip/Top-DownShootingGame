using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;

    private float speed = 10;
    private float damage = 1;

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
            OngitObject(hit);
        }
    }

    void OngitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent <IDamageable>();
        if(damageableObject != null)
        {
            damageableObject.TakeHit(damage, hit); 
        }

        Destroy(gameObject);
    }
}
