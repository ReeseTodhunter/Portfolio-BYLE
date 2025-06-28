using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSlimeBulletHell : BTNode
{
    /*
     * This node allows the slime boss to fire a bullet hell inspired amount of projectiles.
     * -Tom
     */
    private GameObject bulletPrefab; //prefab for projectile
    private int projectilesPerLayer; //projectiles per bullet hell layer
    private int layerCount; //amount of layers
    private float speed, damage; //speed and damage of projectiles
    private string animName = ""; // animation that plays when shoot
    private float delay = 0, delayTimer = 0; //delay before shooting
    private float timeBetweenLayers = 0, layerTimer = 0; //interval between layers shot
    private int currLayer = 0; //current layer
    private float nodeDelay; //cooldown
    private float lastTimeUsed = 0; //last time the node was used
    Quaternion rot;

    //constructor
    public BTSlimeBulletHell(BTAgent _agent, GameObject _prefab, int _projectilesPerLayer, int _layers, float _speed, float _damage, string _animName = "", float _delay = 0, float _timeBetweenLayers = 0.33f, float _cooldown = 5)
    {
        agent = _agent;
        bulletPrefab = _prefab;
        projectilesPerLayer = _projectilesPerLayer;
        layerCount = _layers;
        speed = _speed;
        damage = _damage;
        animName = _animName;
        delay = _delay;
        nodeDelay = _cooldown;
        timeBetweenLayers = _timeBetweenLayers;
    }
    public override NodeState Evaluate(BTAgent agent) //evaluate node output
    {
        if(Time.time - lastTimeUsed < nodeDelay) { return NodeState.FAILURE;} //still cooling down
        agent.SetCurrentAction(this);
        if(animName != "") //play anim
        {
            agent.GetComponent<Animator>().Play(animName);
        }
        delayTimer = 0;
        rot = Quaternion.LookRotation(PlayerController.instance.transform.position - agent.transform.position, Vector3.up);
        layerTimer = timeBetweenLayers;
        currLayer = 0;
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent agent) //update node
    {
        delayTimer += Time.deltaTime;
        if(delayTimer < delay){return;}
        layerTimer += Time.deltaTime;
        if(layerTimer < timeBetweenLayers){return;}
        layerTimer = 0;
        //Get rotation of projectile
        float offset = 180;
        Vector3 eulerRot = rot.eulerAngles;
        eulerRot.y -= offset;
        Quaternion startRot = Quaternion.Euler(eulerRot);
        float angleInterval = 360 / projectilesPerLayer;
        for(int i = 0; i < projectilesPerLayer; i++)
        {
            Vector3 projectileRot = eulerRot + (i * Vector3.up * angleInterval);
            projectileRot.x = 0;
            projectileRot.z = 0;
            Quaternion ProjectileQuat = Quaternion.Euler(projectileRot);
            //Vector3 spawnPos = agent.transform.position;
            Vector3 spawnPos = agent.projectileSpawn.position;
            spawnPos.y = 1f;
            GameObject projectile = GameObject.Instantiate(bulletPrefab, spawnPos, ProjectileQuat);
            projectile.GetComponent<Projectile>().Init(speed,2.5f,15,damage,agent.gameObject);
        }
        currLayer ++;
        if(currLayer >= layerCount)
        {
            ExitNode();
        }
    }
    private void ExitNode()
    {
       // agent.SetMovementEnabled(true);
        lastTimeUsed = Time.time;
        agent.ClearCurrentAction();
    }
}
