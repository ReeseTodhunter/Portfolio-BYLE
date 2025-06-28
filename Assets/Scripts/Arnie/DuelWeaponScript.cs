using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DuelWeaponScript : ProjectileWeapon
{
    public Transform spawnLocationLeft;
    public Transform spawnLocationRight;
    private bool isShooting = false;


    public GameObject model;
    public Transform stowedTransform;
    public Transform initialModelTransform;

    public bool isShootLeft = false;
    public bool isShootRight = false;

    public float shootLeftTimer = 0;
    public float shootRightTimer = 0;

    public override void OnFireOneDown()
    {
        if (isShootLeft) { return; }
        isShootLeft = true;

    }

    public override void OnFireTwoDown()
    {
        if (isShootRight) { return; }
        isShootRight = true;
    }

    public override void OnReloadDown()
    {
        base.OnReloadDown();
        isShooting = false;
    }


    void Update()
    {
        shootRightTimer += Time.deltaTime;
        shootLeftTimer += Time.deltaTime;

        switch (currState)
        {
            case WeaponState.DROPPED:
                Idle();
                model.transform.position = initialModelTransform.transform.position;
                model.transform.eulerAngles = initialModelTransform.transform.eulerAngles;
                break;
            case WeaponState.STOWED:
                model.transform.position = stowedTransform.transform.position;
                model.transform.eulerAngles = stowedTransform.transform.eulerAngles;
                break;
            case WeaponState.EQUIPPED:
                model.transform.position = initialModelTransform.transform.position;
                model.transform.eulerAngles = initialModelTransform.transform.eulerAngles;

                UpdateProjectileWeapon();

                break;
        }
    }

    public override void OnFireOneHeld()
    {


        if (currReloadState != reloadState.READY) { return; }
        if (currMagSize <= 0)
        {
            OnReloadDown();
            return;
        }

        if (shootLeftTimer > rateOfFire)
        {
            spawnLocation = spawnLocationLeft;
            isShootLeft = false;
            shootLeftTimer = 0;
            if (shootAnimName != "")
            {
                animator.speed = 1 / (rateOfFire * (PlayerController.instance.GetModifier(ModifierType.RateOfFire)));
                animator.Play(shootAnimName);
            }
            base.OnShoot();
        }
        else
        {
            isShootLeft = false;
        }

        MagazineUIController.instance.RefreshMagazineUI(magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize)), currMagSize);
    }

    public override void OnFireTwoHeld()
    {
        if (currReloadState != reloadState.READY) { return; }
        if (currMagSize <= 0)
        {
            OnReloadDown();
            return;
        }

        if (shootRightTimer > rateOfFire)
        {
            spawnLocation = spawnLocationRight;
            isShootRight = false;
            shootRightTimer = 0;
            if (shootAnimName != "")
            {
                animator.speed = 1 / (rateOfFire * (PlayerController.instance.GetModifier(ModifierType.RateOfFire)));
                animator.Play(shootAnimName);
            }
            base.OnShoot();
        }
        else
        {
            isShootRight = false;
        }

        MagazineUIController.instance.RefreshMagazineUI(magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize)), currMagSize);
    }
}
