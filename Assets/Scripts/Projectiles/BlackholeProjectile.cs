using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackholeProjectile : Projectile
{
    private float radius;
    private float pullForce;
    private float blackHoleTimer;
    private float tickTimer;
    private float hitRadius;

    private float time;

    private bool active;
    private List<BTAgent> enemies = null;

    public Projectile InitBlackhole(float a_radius, float a_pullForce, float a_blackHoleTimer, float a_tickTimer, float a_hitRadius)
    {
        radius = a_radius;
        pullForce = a_pullForce;
        blackHoleTimer = a_blackHoleTimer;
        tickTimer = a_tickTimer;
        hitRadius = a_hitRadius;
        active = false;

        return this;
    }

    protected override void Update()
    {
        //If the blackhole hasn't been activated run normally
        if (!active) { base.Update(); return; }

        enemies = EnemySpawningSystem.instance.GetEnemies();

        foreach (BTAgent agent in enemies)
        {
            //If the agent is within range of the blackhole
            if (Vector3.Distance(transform.position, agent.transform.position) <= radius)
            {
                //Raycast to check the agent isn't obscured by an object
                if (Physics.Raycast(transform.position, (agent.transform.position - transform.position).normalized, out var hit, radius, LayerMask.GetMask("Untraversable", "Enemy", "SeeThrough"), QueryTriggerInteraction.Ignore))
                {
                    //if not obscured
                    if (hit.collider.gameObject == agent.gameObject)
                    {
                        if ((Vector3.Distance(transform.position, agent.transform.position) <= hitRadius) && (time >= tickTimer))
                        {
                            time = 0;
                            agent.Damage(GetCurrentDamage());
                        }
                        //pull enemy towards the blackhole
                        Vector3 pullDirection = (transform.position - agent.transform.position).normalized;
                        agent.transform.position += pullDirection * pullForce * Time.unscaledDeltaTime;
                    }
                }
            }
        }

        if (time < tickTimer)
        {
            time += Time.deltaTime;
        }

        // Check if projectile has exceeded its lifetime if so destroy the projectile
        if (timer >= lifetime) DestroyProjectile(); 

        // Update timer
        if (isPlayer) timer += Time.unscaledDeltaTime;
        else timer += Time.deltaTime;
    }

    public override void DestroyProjectile(GameObject projectileDestroyer = null)
    {
        //If the blackhole is active destroy the projectile when destroy is next called
        if (active) { base.DestroyProjectile(); return; }

        active = true;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (forceCollide || (other.gameObject != parentObject && !other.gameObject.CompareTag(parentTag) && !other.TryGetComponent(out Projectile _proj) && !other.isTrigger && other.gameObject.layer != 6))
        {
            // If collider is a dead enemy, pass through and ignore
            if (other.TryGetComponent(out BTAgent _agent))
            {
                if (!_agent.isAlive()) { return; } // Goes through dead AIs
            }

            //If the blackhole hasn't been activated, activate blackhole and increase it's lifetime
            if (!active) 
            { 
                active = true;
                lifetime += blackHoleTimer;
                if (impactAudio.Count > 0)
                {
                    this.GetComponent<AudioSource>().clip = impactAudio[Random.Range(0, impactAudio.Count)];
                    this.GetComponent<AudioSource>().volume = GameManager.GMinstance.FXVolume;
                    this.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                    this.GetComponent<AudioSource>().Play();
                }
                return; 
            }

            //if (other.gameObject.TryGetComponent<BTAgent>(out BTAgent agent))
            //{
            //    agent.Damage(GetCurrentDamage());
            //}
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.yellow;
        if (enemies != null)
        {
            if (enemies.Count > 0)
            {
                foreach (BTAgent agent in enemies)
                {
                    if (Vector3.Distance(transform.position, agent.transform.position) <= radius)
                    {
                        //Draw a line to each enemy within range
                        Gizmos.DrawLine(transform.position, agent.transform.position);
                    }
                }
            }
        }
    }
}