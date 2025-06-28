using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjWeaponTest : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] Vector3 weaponOffset = Vector3.zero;
    [SerializeField] float rateOfFire = 0.2f;

    [SerializeField] float speed;
    [SerializeField] float accel;
    [SerializeField] float lifetime;
    [SerializeField] float dmg;

    [SerializeField] int pierce;

    public float force;

    //[SerializeField] int bounce = 3;

    //[SerializeField] GameObject chick;
    //[SerializeField] float orbitRadius = 5.0f;

    //[SerializeField] float returnDistance;

    [SerializeField] GameObject shatterProjectile;

    float rofTimer = 0.0f;

    void Update()
    {
        rofTimer -= Time.deltaTime;
        if (Input.GetKey(KeyCode.F) && rofTimer <= 0.0f)
        {
            Instantiate(projectile, transform.position + (transform.rotation * weaponOffset), transform.rotation).GetComponent<ShatterProjectile>()
                .InitShards(shatterProjectile, speed, accel, dmg/2, lifetime, 16, 50.0f)
                .Init(speed, accel, lifetime, dmg, gameObject)
                .SetPierce(pierce, 0.5f);
            //    .SetFalloff(true, 5.0f, 15.0f, dmg / 2);

            rofTimer = rateOfFire;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayerController.instance.KnockbackFromPos(transform.position, force);
        }
    }
}
