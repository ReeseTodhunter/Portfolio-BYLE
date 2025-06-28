using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class GooseWeapon : ProjectileWeapon
{
    public GameObject chickPrefab;
    public float orbitRadius = 3;
    public GameObject floatingTextPrefab;
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

        if (spawnLocation.gameObject.TryGetComponent<VisualEffect>(out VisualEffect fx))
        {
            fx.Play();
        }
        //Play weapon fire audio
        PlayAudio(fireAudioClips);
        float spread = Random.Range(-spreadInDegrees/2,spreadInDegrees/2);
        Vector3 angle = spawnLocation.transform.eulerAngles;
        angle.y += spread;
        projectile.transform.eulerAngles = angle;
        projectile.GetComponent<EggProjectile>().Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel, lifetime,
            dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject)
            .SetCrit(GetIsCrit(), critMultiplier)
            .SetPierce((int)PlayerController.instance.GetModifier(ModifierType.Pierce), 1.0f);
        projectile.GetComponent<EggProjectile>().InitChicks(chickPrefab,orbitRadius);
        currMagSize--;
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
        if (floatingTextPrefab == null) { return; }
        GameObject text = Instantiate(floatingTextPrefab, muzzleFlash.transform.position, Quaternion.identity);
        text.GetComponent<ItemPickupFade>().SetText("HONK", EffectType.None);
        return;
    }
}
