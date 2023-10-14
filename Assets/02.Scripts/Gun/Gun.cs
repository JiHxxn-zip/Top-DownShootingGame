using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single }
    [SerializeField] private FireMode fireMode;

    [SerializeField] private Transform[] projectileSpawn;
    [SerializeField] private Projectile projectile;
    [SerializeField] private float msBetweenShots = 100;
    [SerializeField] private float muzzleVelocity = 35;

    [Header("[점사 모드]")]
    [SerializeField] private int burstCount;

    [Header("[Shell(포탄 이펙트)]")]
    [SerializeField] private Transform shell;
    [SerializeField] private Transform shellEjection;

    [Header("[섬광 이펙트]")]
    [SerializeField] private Muzzleflash muzzleflash;

    private float nextShotTime;

    private bool isTriggerReleaseSinceLastShot;

    // 버스트모드에서 남은 샷
    private int shotsRemainingInBurst;


    private void Start()
    {
        shotsRemainingInBurst = burstCount;
    }

    private void Shoot()
    {
        if(Time.time > nextShotTime)
        {
            if(fireMode == FireMode.Burst) // burstCount 수 만큼 점사모드
            {
                if(shotsRemainingInBurst == 0)
                {
                    return;
                }

                shotsRemainingInBurst--;
            }
            else if(fireMode == FireMode.Single) // 단발모드
            {
                if(!isTriggerReleaseSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                nextShotTime = Time.time + msBetweenShots / 1000;

                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }

            // Shell + muzzleflash
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleflash.Activate();
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        isTriggerReleaseSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        isTriggerReleaseSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}
