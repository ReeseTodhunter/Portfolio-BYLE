using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTQuadShot : BTNode
{
    /*
        Behaviour Tree Effector node, Will attempt to instantiate and fire multiple projectiles at
        the player.
        -Tom
    */
    private GameObject prefab; //Projectile prefab
    private float projectileCount, currCount = 0; //Max amount of projectiles, current count of projectiles
    private float shotInterval, timer = 0; //Time Interval between instantiating projectiles, timer to measure that interval
    private float damage, speed; //Damage and speed of the projectile
    private float cooldown, timeSinceLastActive = 0; //Cooldown of node and last time it was activated
    private string animName; //Name of animation that is played at the start of this node
    private float spread = 0; //Spread in degrees of projectiles

    private Transform projPos1, projPos2, projPos3, projPos4;
    public BTQuadShot(GameObject _projectilePrefab, Transform _ProjPos1, Transform _ProjPos2, Transform _ProjPos3, Transform _ProjPos4, float _projectileCount, float _shotInterval, float _damage, float _speed, float _nodeCooldown = 3, string _animName = "", float _spreadInDegrees = 15) //Constructor
    {
        //Set all variables
        prefab = _projectilePrefab;
        projectileCount = _projectileCount;
        shotInterval = _shotInterval;
        damage = _damage;
        speed = _speed;
        cooldown = _nodeCooldown;
        animName = _animName;
        spread = _spreadInDegrees;
        projPos1 = _ProjPos1;
        projPos2 = _ProjPos2;
        projPos3 = _ProjPos3;
        projPos4 = _ProjPos4;
    }
    public override NodeState Evaluate(BTAgent _agent) //Evaluate node
    {
        agent = _agent;
        if (Time.time - timeSinceLastActive < cooldown) { return NodeState.FAILURE; } //Cooldown not finished
        agent.SetCurrentAction(this); //Lock action tree into this node
        return NodeState.SUCCESS; //return true
    }
    public override void UpdateNode(BTAgent agent) //Update node
    {
        if (animName != "") //Play animation if its not null
        {
            agent.GetComponent<Animator>().Play(animName);
        }

        if (currCount >= projectileCount) //Exit node if all projectiles shot
        {
            ExitNode();
            return;
        }
        timer += Time.deltaTime; //Incrememnt timer for shooting interval
        
        if (timer > shotInterval) //If interval met, shoot new projectile
        {
            timer = 0; //reset timer 
            
            //FireProjectile(); //Fire projectile

            FireProjectiles(projPos1);
            FireProjectiles(projPos2);
            FireProjectiles(projPos3);
            FireProjectiles(projPos4);

            currCount++; //Incremement projectile count
        }
    }
    private void FireProjectile() //Fire projectile method
    {
        GameObject newProjectie = GameObject.Instantiate(prefab, agent.projectileSpawn.position, agent.projectileSpawn.rotation); //Instantiate new projectile
        newProjectie.GetComponent<Projectile>().Init(speed, 0, 10, damage, agent.gameObject); //init projectile script
        Vector3 pos = newProjectie.transform.position; //Get position of shoot transform
        pos.y = 1; //set y-pos to 1
        float bulletSpread = Random.Range(-spread, spread); //Get random spread
        newProjectie.transform.position = pos; // set position of projectile
        Vector3 eulerRot = newProjectie.transform.rotation.eulerAngles; //Get rotation
        eulerRot.y += bulletSpread; //add spread
        newProjectie.transform.eulerAngles = eulerRot; //apply spread
        timeSinceLastActive = Time.time; //Set cooldown
    }
    private void FireProjectiles(Transform ProjPos) //Fire projectile method
    {
        
        GameObject newProjectie = GameObject.Instantiate(prefab, ProjPos.position, ProjPos.rotation); //Instantiate new projectile
        newProjectie.GetComponent<Projectile>().Init(speed, 0, 10, damage, agent.gameObject); //init projectile script
        Vector3 pos = newProjectie.transform.position; //Get position of shoot transform
        pos.y = 1; //set y-pos to 1
        float bulletSpread = Random.Range(-spread, spread); //Get random spread
        newProjectie.transform.position = pos; // set position of projectile
        Vector3 eulerRot = newProjectie.transform.rotation.eulerAngles; //Get rotation
        eulerRot.y += bulletSpread; //add spread
        newProjectie.transform.eulerAngles = eulerRot; //apply spread
        timeSinceLastActive = Time.time; //Set cooldown
    }

    private void ExitNode() //Exit node
    {
        currCount = 0; //reset counts
        timer = 0;
        agent.ClearCurrentAction(); //Unlock action tree
        return;
    }
}

