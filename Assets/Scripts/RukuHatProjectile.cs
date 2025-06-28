using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class RukuHatProjectile : Projectile
{
    private BTRukuHatThrow throwController;
    private float returnDistance = 0.0f; // How far the projectile will travel before arcing back
    private float turnTime = 0.1f; // Turn time. Magic numbers are cringe but it's best this stays the same
    private float turnVelocity; // I don't know what this does I'll be honest I didn't make the original boomerang code
    private bool returning = false; // When true the projectile
    private enum projectileState
    {
        leaving,
        returning
    }
    private projectileState currState = projectileState.leaving;
    public void InitHat(BTRukuHatThrow _throwController, float _returnDistance)
    {
        throwController = _throwController;
        returnDistance = _returnDistance;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        // Check if the collider is not: on the same team as the parent, not a projectile, not a trigger and not the floor
        if (other.gameObject != parentObject && !other.gameObject.CompareTag(parentTag) && !other.TryGetComponent(out Projectile _proj) && !other.isTrigger && other.gameObject.layer != 6)
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

                // Any on-damage effects
                if (parentCharacter != null) parentCharacter.OnDamageDealt(GetCurrentDamage());

                // Play impact VFX
                if (impactVFX != null && ((pierceCount == pierce) || impactVFXOnPierce))
                {
                    GameObject impact = Instantiate(impactVFX, transform.position, transform.rotation);
                    impact.GetComponent<VisualEffect>().Play();

                    //Play character damage Audio
                    if (impactAudio != null)
                    {
                        //if(impact.TryGetComponent<AudioSource>(out AudioSource source))
                        //{
                        //    //Please change me
                        //    source.clip = impactAudio[Random.Range(0, impactAudio.Count)];
                        //    //impact.GetComponent<AudioSource>().time = 0.3f;
                        //    source.Play();
                        //}
                    }
                }

                // Kill projectile or update pierce counters (depending on what state the projectiles pierce currently is)
                if (pierceCount == pierce) currState = projectileState.returning;
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
                    
                    //Play impact Audio
                    if (impactAudio.Count > 0)
                    {
                        impact.GetComponent<AudioSource>().clip = impactAudio[Random.Range(0, impactAudio.Count)];
                        impact.GetComponent<AudioSource>().Play();
                    }
                }
                currState = projectileState.returning;
            }
        }
    }
    
    protected override void ProjectileUpdate()
    {
        // Check if projectile has moved far enough to start returning
        if (Vector3.Distance(spawnPos, transform.position) > returnDistance || currState == projectileState.returning) returning = true;

        if (returning && parentObject != null)
        {
            // Get position of projectile firer
            Vector3 targetPos = parentObject.transform.position;

            // Turn boomerang slowly to return to the projectile firer
            Vector2 temp = new Vector2(targetPos.x - transform.position.x, targetPos.z - transform.position.z);

            float targetAngle = Mathf.Atan2(temp.x, temp.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // If projectile has reached target destroy self
            if (Vector3.Distance(transform.position, targetPos) < 3f) DestroyProjectile();
        }
    }
    public bool isReturning()
    {
        return currState == projectileState.returning ? true : false;
    }
}
