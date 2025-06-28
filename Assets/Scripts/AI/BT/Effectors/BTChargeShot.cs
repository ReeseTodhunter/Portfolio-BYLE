using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTChargeShot : BTNode
{
    /*
        This Node allows the ai to perform a charge shot.
        The AI shoots a projectile, but has a duration that
        it has charge up the attack. Furthermore, the attack
        has the options to lock the agent in place, and 
        use a line renderer for a laser pointer
    */
    float timeSinceLastShot = 0; //cooldown start time
    float coolDown; //cooldown duration
    private GameObject prefab; //prefab for projectile
    private float velocity, acceleration, damage, chargeDuration; //stats for projectile and duration of charge
    private float chargeTimer = 0; //timer to measure charge duration
    private bool hasLaserSight = false, locksMovement = false; //whether or not the ai needs a line of sight, and whether the node locks movement
    private LineRenderer laser; //laser sight prefab
    private Material projectileMat; //material for projectile
    private string animName; //animation that ai plays
    //constructor
    public BTChargeShot(Projectiles _projectile, ProjectileMats _mat, float _velocity, float _acceleration, float _damage, float _coolDownTimer, float _chargeDuration, bool _locksMovement = true, bool _laserSight = false, string _animName = "")
    {
        prefab = ProjectileLibrary.instance.GetProjectile(_projectile);
        projectileMat = ProjectileLibrary.instance.GetMaterial(_mat);
        velocity = _velocity;
        acceleration = _acceleration;
        damage = _damage;
        coolDown = _coolDownTimer;
        chargeDuration = _chargeDuration;
        locksMovement = _locksMovement;
        hasLaserSight = _laserSight;
        animName = _animName;
    }
    public override NodeState Evaluate(BTAgent agent) //Get the output value of the node
    {
        if(Time.time - timeSinceLastShot < coolDown){return NodeState.FAILURE;} //if cooldown not finished, return
        if(locksMovement) //lock movement if enabled
        {
            agent.SetMovementEnabled(false); 
        }
        if(hasLaserSight) //Instantiate laser sight if enabled
        {
            laser = agent.gameObject.AddComponent<LineRenderer>();
            laser.material = ProjectileLibrary.instance.GetMaterial(ProjectileMats.LASERPOINTER_MAT);
            laser.startWidth = 0.1f;
            laser.endWidth = 0.1f;
            laser.positionCount = 2;
        }
        agent.SetCurrentAction(this); //lock action tree to this node
        chargeTimer = 0;
        if(animName != "") //play animation
        {
            agent.GetComponent<Animator>().Play(animName);
        }
        return NodeState.SUCCESS; //return true
    }
    public override void UpdateNode(BTAgent agent) //update node
    {
        chargeTimer += Time.deltaTime;
        if(hasLaserSight && laser != null) //update laser sight positions if enabled
        {
            laser.SetPosition(0,agent.projectileSpawn.position); //set first position to agent pos
            Vector3 endPos;
            RaycastHit hit;
            if(Physics.Raycast(agent.transform.position, agent.transform.forward, out hit,99, 1 << 8 | 1<< 7, QueryTriggerInteraction.Ignore)) //raycast to player, set that position as second laser point
            {
                endPos = hit.point;
            }
            else
            {
                endPos = agent.projectileSpawn.position + agent.transform.forward * 100;
            }
            laser.SetPosition(1,endPos);
        
            //Update laser colour
            float percentage = chargeTimer / chargeDuration;
            laser.material.SetFloat("_timer",percentage);
        }
        if(chargeTimer < chargeDuration){return;} //if charge duration not met, return
        agent.SetMovementEnabled(true); //enabled movement
        GameObject projectile = GameObject.Instantiate(prefab, agent.projectileSpawn.position, agent.projectileSpawn.rotation); //instantiate projectile
        projectile.GetComponent<BasicProjectile>().Init(velocity,acceleration,10,damage,"Enemy"); //init projectile
        if(laser != null){GameObject.Destroy(laser);} //if laser not null, destroy it
        agent.ClearCurrentAction(); //unlock tree
        timeSinceLastShot = Time.time;
        return;
    }
    public override void DeInitialiseNode(BTAgent agent) //destroy laser if agent dies
    {
        if(laser == null) { return; }
        GameObject.Destroy(laser);
    }
}
