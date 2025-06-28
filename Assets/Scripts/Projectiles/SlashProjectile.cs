using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class SlashProjectile : Projectile
{
    public GameObject slashVFX;
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
                chara.Damage(GetCurrentDamage(),false,true,false,EffectType.None,isCrit);
                GameObject slash = Instantiate(slashVFX,PlayerController.instance.transform.position,PlayerController.instance.transform.rotation);
                slash.GetComponent<VisualEffect>().SetBool("IsCrit",isCrit);
                float rndZ = Random.Range(-30,30);
                Vector3 euler = slash.transform.eulerAngles;
                euler.z = rndZ;
                slash.transform.eulerAngles = euler;
                // Play impact VFX
                if (impactVFX != null && ((pierceCount == pierce) || impactVFXOnPierce))
                {
                    GameObject impact = Instantiate(impactVFX, transform.position, transform.rotation);
                    impact.GetComponent<VisualEffect>().Play();
                }

                // Kill projectile or update pierce counters (depending on what state the projectiles pierce currently is)
                if (pierceCount == pierce) DestroyProjectile();
                else if (pierceCount < pierce) pierceCount++;
            }

            // Collider hit something else (probably a wall)
            else if (pierce != -2)
            {
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
}
