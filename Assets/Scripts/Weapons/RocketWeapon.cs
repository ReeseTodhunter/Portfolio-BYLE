using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RocketWeapon : ProjectileWeapon
{
    private GameObject currentProjectile;
    public GameObject explosionPrefab;
    public float explosionRadius;
    public GameObject rocketObject;
    protected override void OnShoot()
    {
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

        currentProjectile.GetComponent<RocketProjectile>().Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel, lifetime, dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
        .SetCrit(GetIsCrit(), critMultiplier)
        .SetPierce((int)PlayerController.instance.GetModifier(ModifierType.Pierce), 1.0f);
        if (spawnLocation.gameObject.TryGetComponent<VisualEffect>(out VisualEffect fx))
        {
            fx.Play();
        }
        //Play weapon fire audio
        PlayAudio(fireAudioClips);
        currentProjectile.GetComponent<RocketProjectile>().InitRocket(explosionPrefab, explosionRadius);
        if (muzzleFlash == null) { return; }
        VisualEffect vfx;
        if (muzzleFlash.TryGetComponent<VisualEffect>(out vfx))
        {
            vfx.Play();
        }
        ParticleSystem pSystem;
        if (muzzleFlash.TryGetComponent<ParticleSystem>(out pSystem))
        {
            pSystem.Play();
        }
        currMagSize--;
        currReloadState = reloadState.FIRING;
        return;
    }
    private void Update()
    {
        switch (currState)
        {
            case WeaponState.DROPPED:
                Idle();
                break;
            case WeaponState.EQUIPPED:
                if (currMagSize > 0)
                {
                    rocketObject.SetActive(true);
                    currReloadState = reloadState.READY;
                }
                else
                {
                    rocketObject.SetActive(false);
                }
                UpdateProjectileWeapon();
                break;
        }
    }
}