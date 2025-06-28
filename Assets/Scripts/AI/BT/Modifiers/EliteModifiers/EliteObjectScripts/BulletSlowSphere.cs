using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSlowSphere : MonoBehaviour
{
    public float bulletSlowAmount;
    private Projectile effectedProjectile;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Projectile>() != null && other.GetComponent<Projectile>().GetParentTag() == "Player")
        {
            Debug.Log("Slowed");
            effectedProjectile = other.GetComponent<Projectile>();
            effectedProjectile.SetVelocity(effectedProjectile.GetVelocity() / bulletSlowAmount);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Projectile>() != null && other.GetComponent<Projectile>().GetParentTag() == "Player")
        {
            effectedProjectile = other.GetComponent<Projectile>();
            effectedProjectile.SetVelocity(effectedProjectile.GetVelocity() * bulletSlowAmount);
        }
    }
}
