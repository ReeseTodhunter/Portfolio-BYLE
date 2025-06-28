using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RangeShotgunWeapon : ProjectileWeapon
{
    private List<GameObject> unexplodedProjectiles = new List<GameObject>();
    public GameObject shardPrefab;
    public float shardSpeed;
    public float shardAcceleration;
    public float shardDamage;
    public float shardLifeTime;
    public int shardCount;
    public float shardRadius;
    public float explosionDistance; // Distance the bullet will travel before exploding

    protected override void OnShoot()
    {
        GameObject currentProjectile;
        // Spawn projectile
        if (poolObjects) // Object pool
        {
            // If index in pool array is empty, instantiate new projectile and add it to the pool array
            if (projPool[poolIndex] == null)
            {
                currentProjectile = Instantiate(projectilePrefab, spawnLocation.position, spawnLocation.rotation);
                projPool[poolIndex] = currentProjectile;
            }
            // If index in pool array isnt empty, reinitialise the projectile
            else
            {
                currentProjectile = projPool[poolIndex];
                currentProjectile.GetComponent<Projectile>().Enable(spawnLocation);
            }
            poolIndex = (poolIndex + 1) % projPool.Length;
        }
        else // Just instantiate if object pooling isnt being used (only applies to katana really)
        {
            currentProjectile = Instantiate(projectilePrefab, spawnLocation.position, spawnLocation.rotation);
        }
        unexplodedProjectiles.Add(currentProjectile);

        currentProjectile.GetComponent<ShatterProjectile>().Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel, lifetime,
            dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
        .SetCrit(GetIsCrit(), critMultiplier)
        .SetPierce((int)PlayerController.instance.GetModifier(ModifierType.Pierce), 1.0f);
        if (spawnLocation.gameObject.TryGetComponent<VisualEffect>(out VisualEffect fx))
        {
            fx.Play();
        }
        //Play weapon fire audio
        PlayAudio(fireAudioClips);
        currentProjectile.GetComponent<ShatterProjectile>().InitShards(shardPrefab,
            shardSpeed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)),
            shardAcceleration,
            shardDamage * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)),
            shardLifeTime,
            shardCount,
            shardRadius,
            false);
        spawnLocation.gameObject.GetComponent<VisualEffect>().Play();
        currMagSize--;
        currReloadState = reloadState.FIRING;
        return;
    }

    private void Update()
    {
        switch (currState) {
            case WeaponState.DROPPED:
                Idle();
                break;
            case WeaponState.EQUIPPED:
                List<GameObject> toRemove = new List<GameObject>();
                foreach (GameObject projectile in unexplodedProjectiles)
                {
                    if (projectile == null) { toRemove.Add(projectile); }
                    else if (projectile.GetComponent<Projectile>().GetDistanceTraveled() >= explosionDistance)
                    {
                        projectile.GetComponent<ShatterProjectile>().Shatter();
                        projectile.GetComponent<ShatterProjectile>().DestroyProjectile();
                        toRemove.Add(projectile);
                    }
                }
                unexplodedProjectiles.RemoveAll(item => toRemove.Contains(item));

                UpdateProjectileWeapon();
                break;
        }
    }
}
