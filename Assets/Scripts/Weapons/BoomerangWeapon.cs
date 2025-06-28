using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class BoomerangWeapon : ProjectileWeapon
{
    public float returnDistance = 10;
    protected override void OnShoot()
    {
        // Spawn projectile
        GameObject projectile;
        if (poolObjects) // Object pool
        {
            // If index in pool array is empty, instantiate new projectile and add it to the pool array
            if (projPool[poolIndex] == null)
            {
                projectile = Instantiate(projectilePrefab, spawnLocation.position, spawnLocation.rotation);
                projPool[poolIndex] = projectile;
            }
            // If index in pool array isnt empty, reinitialise the projectile
            else
            {
                projectile = projPool[poolIndex];
                projectile.GetComponent<Projectile>().Enable(spawnLocation);
            }
            poolIndex = (poolIndex + 1) % projPool.Length;
        }
        else // Just instantiate if object pooling isnt being used (only applies to katana really)
        {
            projectile = Instantiate(projectilePrefab, spawnLocation.position, spawnLocation.rotation);
        }

        spawnLocation.gameObject.GetComponent<VisualEffect>().Play();
        projectile.GetComponent<BoomerangProjectile>()
            .Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel, lifetime, 
            dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
            .SetCrit(GetIsCrit(),critMultiplier)
            .SetPierce( -1, 1.0f);
        if (spawnLocation.gameObject.TryGetComponent<VisualEffect>(out VisualEffect fx))
        {
            fx.Play();
        }
        //Play weapon fire audio
        PlayAudio(fireAudioClips);
        projectile.GetComponent<BoomerangProjectile>().InitBoomerang(returnDistance);
        currMagSize--;
        currReloadState = reloadState.FIRING;
        return;
    }
}
