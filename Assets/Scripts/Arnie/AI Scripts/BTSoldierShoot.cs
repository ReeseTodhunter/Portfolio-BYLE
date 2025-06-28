using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSoldierShoot : BTNode
{
    private GameObject prefab;
    private Material mat;
    private float projectileCount, currCount = 0;
    private float shotInterval, timer = 0;
    private float damage, speed;
    private float cooldown, timeSinceLastActive = 0;
    private string animName;
    private string finalAnim;
    private float spread = 0;
    public BTSoldierShoot(GameObject _projectilePrefab, float _projectileCount, float _shotInterval, float _damage, float _speed, float _nodeCooldown = 3, string _animName = "", float _spreadInDegrees = 15, string _finalAnim = "")
    {
        prefab = _projectilePrefab;
        projectileCount = _projectileCount;
        shotInterval = _shotInterval;
        damage = _damage;
        speed = _speed;
        cooldown = _nodeCooldown;
        animName = _animName;
        finalAnim = _finalAnim;
        spread = _spreadInDegrees;
    }
    public override NodeState Evaluate(BTAgent _agent)
    {
        agent = _agent;
        if (Time.time - timeSinceLastActive < cooldown) { return NodeState.FAILURE; }
        agent.SetCurrentAction(this);
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent agent)
    {
        if (currCount >= projectileCount)
        {
            agent.GetComponent<Animator>().Play(finalAnim);
            ExitNode();
            return;
        }
        timer += Time.deltaTime;
        if (timer > shotInterval)
        {
            timer = 0;
            if (animName != "")
            {
                agent.GetComponent<Animator>().Play(animName);
            }
            FireProjectile();
            currCount++;
        }
    }
    private void FireProjectile()
    {
        GameObject newProjectie = GameObject.Instantiate(prefab, agent.projectileSpawn.position, agent.projectileSpawn.rotation);
        //newProjectie.GetComponent<MeshRenderer>().material = mat;
        newProjectie.GetComponent<Projectile>().Init(speed, 0, 10, damage, agent.gameObject);
        Vector3 pos = newProjectie.transform.position;
        pos.y = 1;
        float bulletSpread = Random.Range(-spread, spread);
        newProjectie.transform.position = pos;
        Vector3 eulerRot = newProjectie.transform.rotation.eulerAngles;
        eulerRot.y += bulletSpread;
        newProjectie.transform.eulerAngles = eulerRot;
        timeSinceLastActive = Time.time;
    }
    private void ExitNode()
    {
        currCount = 0;
        timer = 0;
        agent.ClearCurrentAction();
        return;
    }
}

