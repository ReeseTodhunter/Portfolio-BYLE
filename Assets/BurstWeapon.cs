using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstWeapon : ProjectileWeapon
{
    public int burstSize;
    public float burstInterval;
    private bool isShooting = false;
    private int currBurstShot = 0;
    private float burstTimer = 0;
    protected override void OnShoot()
    {
        if(isShooting){return;}
        isShooting = true;
        currBurstShot = 0;
        burstTimer = 0;
    }
    public override void OnReloadDown()
    {
        base.OnReloadDown();
        isShooting = false;
    }
    void Update()
    {
        switch (currState)
        {
            case WeaponState.DROPPED:
                Idle();
                break;
            case WeaponState.EQUIPPED:
                UpdateProjectileWeapon();
                if (!isShooting) { return; }
                if (currBurstShot >= burstSize) { isShooting = false; return; }
                burstTimer += Time.deltaTime;
                if (burstTimer < burstInterval) { return; }
                burstTimer = 0;
                currBurstShot++;
                base.OnShoot();
                break;
        }
    }
}
