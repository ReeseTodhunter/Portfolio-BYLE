using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceProjectile : Projectile
{
    protected int bounces = 0;

    public Projectile InitBounce(int a_bounces)
    {
        bounces = a_bounces;
        return this;
    }

    public override void DestroyProjectile(GameObject projectileDestroyer = null)
    {
        // The bounces works by overriding destroy projectile and making it bounce instead
        // The original destroy is only called when the projectiles lifetime is up or it has run out of bounces
        if (timer >= lifetime || bounces <= 0) base.DestroyProjectile(); // Destroy projectile if no bounces or expired

        bounces--; // Decrease bounce count
        pierceCount = 0; // Reset pierce
        if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            transform.LookAt(transform.position + Vector3.Reflect(transform.forward, hitInfo.normal)); // Turn projectile to face opposite direction to raycast
        }
    }
}
