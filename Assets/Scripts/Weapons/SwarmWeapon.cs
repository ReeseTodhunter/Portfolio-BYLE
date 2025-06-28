using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SwarmWeapon : ProjectileWeapon
{
    private GameObject currentProjectile;
    public float swarmRange = 5.0f;
    public float minTargetRange = 5.0f;
    public int basePierce = 5;
    protected override void OnShoot()
    {
        // Spawn projectile
        if (poolObjects) // Object pooling
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

        currentProjectile.GetComponent<SwarmProjectile>().Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel, lifetime, dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
        .SetCrit(GetIsCrit(), critMultiplier)
        .SetPierce((int)PlayerController.instance.GetModifier(ModifierType.Pierce) + basePierce, 1.0f);
        if (spawnLocation.gameObject.TryGetComponent<VisualEffect>(out VisualEffect fx))
        {
            fx.Play();
        }
        //Play weapon fire audio
        PlayAudio(fireAudioClips);
        currentProjectile.GetComponent<SwarmProjectile>().InitSwarm(swarmRange, minTargetRange);
        spawnLocation.gameObject.GetComponent<VisualEffect>().Play();
        currMagSize--;
        currReloadState = reloadState.FIRING;
        return;
    }
}
