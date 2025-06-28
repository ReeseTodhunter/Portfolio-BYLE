using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigunWeapon : ProjectileWeapon
{

    public GameObject model;
    public Transform stowedTransform;
    public Transform initialModelTransform;



    void Update()
    {
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

                //base.OnShoot();
                break;
        }
    }
}

