using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RocketProjectile : Projectile
{
    protected GameObject explosionEffect;
    protected float radius;

    public Projectile InitRocket(GameObject a_explosionEffect, float a_radius)
    {
        explosionEffect = a_explosionEffect;
        radius = a_radius;

        return this;
    }

    protected override void OnTriggerEnter(Collider other) // Called when something enters the projectiles trigger zone
    {
        // Check if the collider is not: on the same team as the parent, not a projectile, not a trigger and not the floor
        if (forceCollide || (other.gameObject != parentObject && !other.gameObject.CompareTag(parentTag) && !other.TryGetComponent(out Projectile _proj) && !other.isTrigger && other.gameObject.layer != 6))
        {
            // If collider is a dead enemy, pass through and ignore
            if (other.TryGetComponent(out BTAgent _agent))
            {
                if (!_agent.isAlive()) { return; } // Goes through dead AIs
            }

            // If collider has a character component
            if (other.TryGetComponent(out Character chara))
            {
                // Damage character
                if (burnEffect) chara.Burn(burnTime, burnDamage);
                if (poisonEffect) chara.Poison(poisonTime, poisonDamage, poisonSpeed);
                Explode();

                // Play impact VFX
                if (impactVFX != null && ((pierceCount == pierce) || impactVFXOnPierce))
                {
                    GameObject impact = Instantiate(impactVFX, transform.position, transform.rotation);
                    impact.GetComponent<VisualEffect>().Play();

                    //Play character damage Audio
                    if (impactAudio != null)
                    {
                        if (impact.TryGetComponent<AudioSource>(out AudioSource source))
                        {
                            //Please change me
                            impact.GetComponent<AudioSource>().clip = impactAudio[Random.Range(0, impactAudio.Count)];
                            //impact.GetComponent<AudioSource>().time = 0.3f;
                            impact.GetComponent<AudioSource>().Play();
                        }
                    }
                }

                // Kill projectile or update pierce counters (depending on what state the projectiles pierce currently is)
                if (pierceCount == pierce) DestroyProjectile(other.gameObject);
                else if (pierceCount < pierce) pierceCount++;
            }

            // Collider hit something else (probably a wall)
            else if (pierce != -2)
            {
                Explode();
                // Play impact VFX
                if (impactVFX != null && ((pierce == 0) || impactVFXOnPierce))
                {
                    GameObject impact = Instantiate(impactVFX, transform.position, transform.rotation);
                    impact.GetComponent<VisualEffect>().Play();
                }

                // Kill projectile unless pierce allows for projectile to travel through walls
                DestroyProjectile();
            }
        }
    }

    public override void DestroyProjectile(GameObject projectileDestroyer = null)
    {
        // If not pooled then just destroy this game object
        if (!isPooled)
        {
            Destroy(gameObject);
        }
        
        Destroy(this.gameObject);
    }

    private void Explode()
    {
        //If there is an explosion effect instantiate the effect
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
            //Play impact Audio
            if (impactAudio.Count > 0)
            {
                explosion.GetComponent<AudioSource>().clip = impactAudio[Random.Range(0, impactAudio.Count)];
                explosion.GetComponent<AudioSource>().volume = GameManager.GMinstance.FXVolume;
                explosion.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                explosion.GetComponent<AudioSource>().Play();
            }
        }

        //Get all collisions in range of the explosion
        Collider[] objects = Physics.OverlapSphere(transform.position, radius);

        //Check if each object is a character and if so damage them
        foreach (Collider obj in objects)
        {
            if (obj.gameObject.TryGetComponent<Character>(out Character character))
            {
                character.Damage(GetCurrentDamage());
            }
        }
        CameraController.instance.ShakeCameraOverTime(0.5f, 1);
    }
}