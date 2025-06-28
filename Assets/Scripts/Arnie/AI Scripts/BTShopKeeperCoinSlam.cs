using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTShopKeeperCoinSlam : BTNode
{
    private GameObject bulletPrefab;
    private int projectilesPerLayer;
    private int layerCount;
    private float speed, damage;
    private string animName = "";
    private float delay = 0, delayTimer = 0;
    private float timeBetweenLayers = 0, layerTimer = 0;
    private int currLayer = 0;
    private float nodeDelay;
    private float lastTimeUsed = 0;
    Quaternion rot;
    public BTShopKeeperCoinSlam(BTAgent _agent, GameObject _prefab, int _projectilesPerLayer, int _layers, float _speed, float _damage, string _animName = "", float _delay = 0, float _timeBetweenLayers = 0.33f, float _cooldown = 5)
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
    public override NodeState Evaluate(BTAgent agent)
    {
        if (Time.time - lastTimeUsed < nodeDelay) { return NodeState.FAILURE; }
        agent.SetCurrentAction(this);
        //agent.SetMovementEnabled(false);
        if (animName != "")
        {
            agent.GetComponent<Animator>().Play(animName);
        }
        delayTimer = 0;
        rot = Quaternion.LookRotation(PlayerController.instance.transform.position - agent.transform.position, Vector3.up);
        layerTimer = timeBetweenLayers;
        currLayer = 0;
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent agent)
    {
        delayTimer += Time.deltaTime;
        if (delayTimer < delay) { return; }
        layerTimer += Time.deltaTime;
        if (layerTimer < timeBetweenLayers) { return; }
        layerTimer = 0;
        //Get rotation of projectile
        float offset = 180;
        Vector3 eulerRot = rot.eulerAngles;
        eulerRot.y -= offset;
        Quaternion startRot = Quaternion.Euler(eulerRot);
        float random = Random.Range(0, 30);
        float angleInterval = 360 / projectilesPerLayer;
        for (int i = 0; i < projectilesPerLayer; i++)
        {
            float rand = Random.Range(0, 180);
            Vector3 projectileRot = eulerRot + ( i * Vector3.up * (angleInterval + random));
            projectileRot.x = 0;
            projectileRot.z = 0 + rand;
            Quaternion ProjectileQuat = Quaternion.Euler(projectileRot);

            Vector3 spawnPos = agent.projectileSpawn.position;
            spawnPos.y = 1f;
            GameObject projectile = GameObject.Instantiate(bulletPrefab, spawnPos, ProjectileQuat);
            projectile.GetComponent<Projectile>().Init(speed, 2.5f, 15, damage, agent.gameObject);
        }
        currLayer++;
        if (currLayer >= layerCount)
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
