using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlasmaShoot : ProjectileWeapon
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

        float spread = Random.Range(-spreadInDegrees / 2, spreadInDegrees / 2);
        Vector3 angle = spawnLocation.transform.eulerAngles;
        angle.y += spread;
        projectile.transform.eulerAngles = angle;
        projectile.GetComponent<Projectile>().Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel, lifetime,
            dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
        .SetCrit(GetIsCrit(), critMultiplier)
        .SetStun(true, 0.5f)
        .SetPierce(-1, 1.0f);
        currMagSize--;

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

        return;
    }
}
