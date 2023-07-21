using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    [Header("[Health]")]
    [SerializeField] private float startinhHealth;

    protected float health;
    protected bool isDead;

    protected virtual void Start()
    {
        health = startinhHealth;
    }

    public void TakeHit(float damage, RaycastHit git)
    {
        health -= damage;

        if(health <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
