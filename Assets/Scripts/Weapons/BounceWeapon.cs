using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class BounceWeapon : ProjectileWeapon
{
    private GameObject currentProjectile;
    public int bounces = 3;
    public int passes = 3;
    protected override void OnShoot()
    {
        //Get random colour and set the muzzle flash to that colour
        Color colour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        if (spawnLocation.gameObject.TryGetComponent<VisualEffect>(out VisualEffect fx))
        {
            fx.Play();
            fx.SetVector4("MainColor", (Vector4)colour);
        }

        //Setup the colour's intensity for the projectiles
        var intensity = (colour.r + colour.g + colour.b) / 3f;
        var factor = 1f / intensity;
        colour = new Color(colour.r * factor, colour.g * factor, colour.b * factor, colour.a);
        for (int i = 0; i < passes; i++)
        {
            colour *= colour;
        }
        if (colour.r > 10) colour.r = 20;
        if (colour.g > 10) colour.g = 20;
        if (colour.b > 10) colour.b = 20;

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

        currentProjectile.GetComponent<BounceProjectile>().Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel, lifetime, dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
        .SetCrit(GetIsCrit(), critMultiplier)
        .SetPierce((int)PlayerController.instance.GetModifier(ModifierType.Pierce), 1.0f);
        if (currentProjectile.gameObject.TryGetComponent<VisualEffect>(out VisualEffect pfx))
        {
            pfx.SetVector4("MainColor", (Vector4)colour);
        }
        //Play weapon fire audio
        PlayAudio(fireAudioClips);
        currentProjectile.GetComponent<BounceProjectile>().InitBounce(bounces);
        spawnLocation.gameObject.GetComponent<VisualEffect>().Play();
        currMagSize--;
        currReloadState = reloadState.FIRING;
        return;
    }
}