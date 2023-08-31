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

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
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
