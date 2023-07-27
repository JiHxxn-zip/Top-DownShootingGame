using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    [Header("[Health]")]
    [SerializeField] private float startinhHealth;

    protected float health;
    protected bool isDead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startinhHealth;
    }

    public void TakeHit(float damage, RaycastHit git)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        isDead = true;

        if(OnDeath != null)
        {
            OnDeath();
        }

        Destroy(gameObject);
    }
}
