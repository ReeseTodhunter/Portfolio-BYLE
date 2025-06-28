using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmProjectile : Projectile
{
    private float senseRange = 0;
    private float minRange = 0;

    private List<BTAgent> enemies = null;
    private BTAgent targetEnemy = null;

    public Projectile InitSwarm(float a_senseRange, float a_minRange)
    {
        senseRange = a_senseRange;
        minRange = a_minRange;
        return this;
    }

    protected override void ProjectileUpdate()
    {
        //Get all alive enemies if there is an enemy spawner
        if (EnemySpawningSystem.instance != null)
        {
            enemies = EnemySpawningSystem.instance.GetEnemies();
        }

        //if there is no current target
        if (targetEnemy == null)
        {
            //Reset the closest position
            float closest = senseRange;
            BTAgent possibleEnemy = null;

            //for every alive enemy
            foreach (BTAgent agent in enemies)
            {
                //If the enemy is within range of the swarm
                if (Vector3.Distance(transform.position, agent.transform.position) <= senseRange)
                {
                    //Raycast to check nothing obscures the enemy
                    if (Physics.Raycast(transform.position, (agent.transform.position - transform.position).normalized, out var hit, senseRange, LayerMask.GetMask("Untraversable", "Enemy", "SeeThrough"), QueryTriggerInteraction.Ignore))
                    {
                        //if unobscured
                        if (hit.collider.gameObject == agent.gameObject)
                        {
                            //Check if the nearby enemy is the closest enemy to the swarm projectile
                            if (Vector3.Distance(transform.position, agent.transform.position) < closest)
                            {
                                //update the closest enemy
                                closest = Vector3.Distance(transform.position, agent.transform.position);
                                targetEnemy = agent;
                            }
                        }
                    }
                }
            }
        }

        //If there is an enemy target
        if (targetEnemy != null && Vector3.Distance(transform.position, targetEnemy.transform.position) > minRange)
        {
            //Look at the targeted enemy
            transform.LookAt(targetEnemy.transform);
        }
    }

    //protected override void DestroyProjectile(GameObject projectileDestroyer = null)
    //{
    //    if (targetEnemy != null && timer < lifetime)
    //    {
    //        Debug.Log("Timer: " + timer);
    //        Debug.Log("Target: " + targetEnemy);
    //        return;
    //    }
    //    Debug.Log("Success");
    //    base.DestroyProjectile(projectileDestroyer);
    //}
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, senseRange);

        Gizmos.color = Color.yellow;
        if (enemies != null)
        {
            if (enemies.Count > 0)
            {
                foreach (BTAgent agent in enemies)
                {
                    if (Vector3.Distance(transform.position, agent.transform.position) <= senseRange)
                    {
                        //Draw a line to each enemy within range
                        Gizmos.DrawLine(transform.position, agent.transform.position);
                    }
                }
            }
        }
    }
}
