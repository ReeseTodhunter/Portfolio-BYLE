using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class ShotgunWeapon : ProjectileWeapon
{
    public float bulletsPerShot = 5;
    protected override void OnShoot()
    {
        spawnLocation.gameObject.GetComponent<VisualEffect>().Play();
        float startYrot = spreadInDegrees * -0.5f;
        float increment = spreadInDegrees / bulletsPerShot;
        for(int i =0; i < bulletsPerShot; i++)
        {
            float yRot = startYrot + increment * i;

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

            Vector3 angles = projectile.transform.eulerAngles;
            angles.y += yRot;
            projectile.transform.eulerAngles = angles;

            //init projectile
            projectile.GetComponent<Projectile>().Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel, lifetime,
            dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
            .SetCrit(GetIsCrit(),critMultiplier)
            .SetPierce((int)PlayerController.instance.GetModifier(ModifierType.Pierce), 1.0f);
        }
        if (spawnLocation.gameObject.TryGetComponent<VisualEffect>(out VisualEffect fx))
        {
            fx.Play();
        }
        //Play weapon fire audio
        PlayAudio(fireAudioClips);
        currMagSize --;

        //Play VFX
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
        currReloadState = reloadState.FIRING;
    }
}
