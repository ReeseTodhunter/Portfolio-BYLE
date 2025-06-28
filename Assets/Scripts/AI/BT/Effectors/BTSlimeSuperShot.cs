using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSlimeSuperShot : BTNode
{
    /*
     * This node allows the slime boss to fire a super projectile that explodes into smaller projectiles on impact
     * -Tom
     */
    private float chargeDuration = 0, chargeTimer = 0; //charge duration of attack
    private float speed, damage = 0; //speed and damage of projectile
    private GameObject projectilePrefab; //prefab of projectile
    private GameObject shardPrefab; //Shard prefab
    private GameObject instance; //instance of projectile
    private float cooldown = 0, cooldownTimer = 0; //cooldown of attack
    private int childProjectiles; //child projectile count
    private string animName = ""; //anim name
    private float lastTimeUsed = 0, nodeCooldown; // last time node activated and the cooldown
    private enum scriptState
    {
        IDLE,
        CHARGING,
        SHOOTING,
        COOLDOWN
    }
    private scriptState currState; //current script state
    private bool spawnProjectileImmedietly; //whether or not the attack is instant

    //Constructor
    public BTSlimeSuperShot(BTAgent _agent, GameObject _prefab, GameObject _shardPrefab, float _chargeDuration, float _speed, float _damage, float _cooldown, int _childProjectiles = 12, string _animName = "", bool _spawnProjectileImmedietly = false, float _nodeCooldown = 6)
    {
        agent = _agent;
        projectilePrefab = _prefab;
        shardPrefab = _shardPrefab;
        speed = _speed;
        damage = _damage;
        cooldown = _cooldown;
        childProjectiles = _childProjectiles;
        animName = _animName;
        spawnProjectileImmedietly = _spawnProjectileImmedietly;
        nodeCooldown = _nodeCooldown;
    }
    public override NodeState Evaluate(BTAgent agent) //Evaluate node
    {
        if (Time.time - lastTimeUsed < nodeCooldown) { return NodeState.FAILURE; } //if still cooling down, return false
        chargeTimer = 0;
        cooldownTimer = 0;
        currState = scriptState.IDLE;
        if(animName != "") //play anim
        {
            agent.GetComponent<Animator>().Play(animName);
        }
        //agent.SetMovementEnabled(false);
        agent.SetCurrentAction(this);
        if(spawnProjectileImmedietly) //fire projectile if spawn immedietly
        {
            instance = GameObject.Instantiate(projectilePrefab,agent.projectileSpawn.position,agent.transform.rotation);
            instance.GetComponent<ShatterProjectile>().Init(0,0,20,damage,agent.gameObject,false);
            instance.GetComponent<ShatterProjectile>().InitShards(shardPrefab,20,2.5f,10,20,childProjectiles,360.0f);
            Vector3 pos = instance.transform.position;
            pos.y = 1;
            instance.transform.position = pos;
        }
        currState = scriptState.CHARGING;
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent agent) //update node
    {
        switch(currState)
        {
            case scriptState.IDLE: //do nothing
                break;
            case scriptState.CHARGING: //charge up
                chargeTimer += Time.deltaTime;
                if(spawnProjectileImmedietly && instance != null)
                {
                    Vector3 pos = agent.transform.position;
                    pos.y = 1;
                    instance.transform.position = pos;
                }
                if(chargeTimer <= chargeDuration){break;}
                currState = scriptState.SHOOTING;
                break;
            case scriptState.SHOOTING:
                Shoot(); //shoot projectile
                currState = scriptState.COOLDOWN;
                break;
            case scriptState.COOLDOWN: //cooldown
                cooldownTimer += Time.deltaTime;
                if(cooldownTimer <= cooldown){break;}
                ExitNode();
                break;
        }
    }
    private void Shoot() //shoot projectile
    {
        if(spawnProjectileImmedietly && instance != null)
        {
            instance.GetComponent<ShatterProjectile>().SetVelocity(speed);
        }
        else
        {
            instance = GameObject.Instantiate(projectilePrefab,agent.projectileSpawn.position,agent.transform.rotation); //instantiate projectile
            instance.GetComponent<ShatterProjectile>().Init(speed,0,20,damage,agent.gameObject,false); //init projectile
            instance.GetComponent<ShatterProjectile>().InitShards(shardPrefab,15,0,10,20,childProjectiles,360.0f); //init projectile shards
            Vector3 pos = instance.transform.position;
            pos.y = 1;
            instance.transform.position = pos;
        }
    }
    private void ExitNode() //exit node
    {
        currState = scriptState.IDLE;
        //agent.SetMovementEnabled(true);
        lastTimeUsed = Time.time;
        agent.ClearCurrentAction();
    }
}
