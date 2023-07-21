using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private Transform weaponHold;

    [SerializeField] private Gun startingGun;

    private Gun equippedGun;


    private void Start()
    {
        if(startingGun != null)
        {
            EqiopGun(startingGun);
        }
    }

    public void EqiopGun(Gun gunToEquip)
    {
        if(equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }

        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation, weaponHold) as Gun;
    }

    public void Shoot()
    {
        if(equippedGun != null)
        {
            equippedGun.Shoot();
        }
    }
}
