using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class ShockRifleWeapon : ProjectileWeapon
{
    protected override void OnShoot()
    {
        GameObject projectile;

        // Object pooling
        if (poolObjects)
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

        // Just instantiate if object pooling isnt being used (only applies to katana really, and that overrides this function anyway)
        else
        {
            projectile = Instantiate(projectilePrefab, spawnLocation.position, spawnLocation.rotation);
        }

        //Play weapon fire audio
        PlayAudio(fireAudioClips);

        float spread = Random.Range(-spreadInDegrees/2,spreadInDegrees/2);
        Vector3 angle = spawnLocation.transform.eulerAngles;
        angle.y += spread;
        projectile.transform.eulerAngles = angle;


        int rand = Random.Range(0, 5);
        if (rand == 4)
        {
            projectile.GetComponent<Projectile>().Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel, lifetime,
            dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
        .SetCrit(GetIsCrit(), critMultiplier)
        .SetPierce((int)PlayerController.instance.GetModifier(ModifierType.Pierce), 1.0f)
        .SetStun(true, 3);
            currMagSize--;
        }
        else
        {
            projectile.GetComponent<Projectile>().Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel, lifetime,
            dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
        .SetCrit(GetIsCrit(), critMultiplier)
        .SetPierce((int)PlayerController.instance.GetModifier(ModifierType.Pierce), 1.0f)
        .SetStun(false, 0);
            currMagSize--;
        }

        

        //Play VFX
        if(muzzleFlash == null) { return; }
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
        return;
    }
}
