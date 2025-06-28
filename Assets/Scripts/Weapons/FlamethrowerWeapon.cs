using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class FlamethrowerWeapon : ProjectileWeapon
{
    public float projectilesPerShot = 3;
    public float burnDamage = 1.5f;
    public float burnDuration = 3;
    protected override void OnShoot()
    {
        for(int i =0; i < projectilesPerShot; i++)
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

            float spread = Random.Range(-spreadInDegrees/2,spreadInDegrees/2);
            Vector3 angle = spawnLocation.transform.eulerAngles;
            angle.y += spread;
            projectile.transform.eulerAngles = angle;
            projectile.GetComponent<Projectile>().Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), lifetime,
            dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
            .SetBurn(true,burnDamage * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), burnDuration)
            .SetCrit(GetIsCrit(),critMultiplier)
            .SetPierce((int)PlayerController.instance.GetModifier(ModifierType.Pierce), 1.0f);
        }
        if (muzzleFlash.gameObject.TryGetComponent<VisualEffect>(out VisualEffect fx))
        {
            fx.Play();
        }
        if (muzzleFlash.gameObject.TryGetComponent<ParticleSystem>(out ParticleSystem pSystem))
        {
            pSystem.Play();
        }
        //Play weapon fire audio
        PlayAudio(fireAudioClips);
        currMagSize--;
        currReloadState = reloadState.FIRING;
    }
}
