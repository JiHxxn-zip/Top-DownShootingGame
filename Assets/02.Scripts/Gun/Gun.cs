using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform muzzle;
    [SerializeField] private Projectile projectile;
    [SerializeField] private float msBetweenShots = 100;
    [SerializeField] private float muzzleVelocity = 35;

    [Header("[Shell(포탄 이펙트)]")]
    [SerializeField] private Transform shell;
    [SerializeField] private Transform shellEjection;

    [Header("[섬광 이펙트]")]
    [SerializeField] private Muzzleflash muzzleflash;

    private float nextShotTime;


    public void Shoot()
    {
        if(Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;

            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);

            // Shell + muzzleflash
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleflash.Activate();
        }
    }
}
