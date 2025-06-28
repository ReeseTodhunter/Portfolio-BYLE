using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SnakeWeapon : ProjectileWeapon
{
    private GameObject currentProjectile;
    protected override void OnShoot()
    {

        // Object pooling
        if (poolObjects)
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

        // Just instantiate if object pooling isnt being used (only applies to katana really, and that overrides this function anyway)
        else
        {
            currentProjectile = Instantiate(projectilePrefab, spawnLocation.position, spawnLocation.rotation);
        }

        //Play weapon fire audio
        PlayAudio(fireAudioClips);

        float spread = Random.Range(-spreadInDegrees / 2, spreadInDegrees / 2);
        Vector3 angle = spawnLocation.transform.eulerAngles;
        angle.y += spread;
        currentProjectile.transform.eulerAngles = angle;
        currentProjectile.GetComponent<Projectile>().Init(speed * (1), accel, lifetime,
            dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
        .SetCrit(GetIsCrit(), critMultiplier)
        .SetPoison(true, 1.0f, 0, 1);
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

    public override void OnFireTwoDown()
    {
        if (currentProjectile != null)
        {
            currentProjectile.GetComponent<SnakeProj>().spawnSnake();
        }
    }
}
