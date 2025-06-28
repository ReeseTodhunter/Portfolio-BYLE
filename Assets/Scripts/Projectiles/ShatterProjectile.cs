using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterProjectile : Projectile
{
    // Shard variables. All other shard projectile variables will be identical to this un-shattered projectile's variables
    private GameObject shardObject = null; // Actual shard game object
    private float shardSpeed = 0.0f; // Speed that the shards will travel
    private float shardAccel = 0.0f; // Acceleration of the shards
    private float shardDamage = 0.0f; // Damage each shard will do
    private float shardLifetime = 0.0f; //How long each shard will exist
    private int shardCount = 1; // Amount of shards spawned upon collision
    private float shatterRadius = 0.0f; // Angle in degrees that the shards will spread out in
    private bool shatterOnDestroy = true; // If true then the projectile will shatter when destroyed

    public Projectile InitShards(GameObject a_object, float a_speed, float a_accel, float a_dmg, float a_lifetime, int a_count, float a_radius, bool a_autoShatter = true)
    {
        shardObject = a_object;
        shardSpeed = a_speed;
        shardAccel = a_accel;
        shardDamage = a_dmg;
        shardLifetime = a_lifetime;
        shardCount = Mathf.Max(1, a_count); // Can't have this value be 0
        shatterRadius = a_radius;
        shatterOnDestroy = a_autoShatter;

        return this;
    }

    public void Shatter(GameObject projectileDestroyer = null)
    {
        // Spawn shards
        float spawnAngle = shatterRadius / shardCount;
        for (int i = 0; i < shardCount; ++i)
        {
            // guy who wrote the comment below is a cunt (me)
            // Does not take into account object pooling yet. Good luck figuring this one out LOL
            Instantiate(shardObject, transform.position, Quaternion.Euler(0, transform.rotation.eulerAngles.y + (spawnAngle * i) - (shatterRadius / 2), 0)).GetComponent<ShardProjectile>()
                .InitIgnoredObject(projectileDestroyer)
                .Init(shardSpeed, shardAccel, shardLifetime, shardDamage, parentObject, false)
                .SetBurn(burnEffect, burnDamage, burnTime)
                .SetPoison(poisonEffect, poisonDamage, poisonSpeed, poisonTime)
                .SetPierce(pierce, pierceFalloff)
                .SetFalloff(isFalloff, falloffDistStart, falloffDistEnd, falloffDamage);
        }
    }

    public override void DestroyProjectile(GameObject projectileDestroyer = null)
    {
        if (shatterOnDestroy) Shatter(projectileDestroyer);
        base.DestroyProjectile();
    }
}
