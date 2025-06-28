using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTFlameThrowerShoot : BTNode
{
    private GameObject prefab;
    private Material mat;
    private float projectileCount, currCount = 0;
    private float shotInterval, timer = 0;
    private float damage, speed;
    private float cooldown, timeSinceLastActive = 0;
    private string animName;
    private float spread = 0;
    public BTFlameThrowerShoot(GameObject _projectilePrefab, float _projectileCount, float _shotInterval, float _damage, float _speed, float _nodeCooldown = 3, string _animName = "", float _spreadInDegrees = 15)
    {
        prefab = _projectilePrefab;
        projectileCount = _projectileCount;
        shotInterval = _shotInterval;
        damage = _damage;
        speed = _speed;
        cooldown = _nodeCooldown;
        animName = _animName;
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
            agent.projectileSpawn.GetComponent<ParticleSystem>().Play();
            agent.projectileSpawn.GetComponentInChildren<ParticleSystem>().Play();
            FireProjectile();
            currCount++;
        }
    }
    private void FireProjectile()
    {
        
        GameObject newProjectie = GameObject.Instantiate(prefab, agent.projectileSpawn.position, agent.projectileSpawn.rotation);
        //newProjectie.GetComponent<MeshRenderer>().material = mat;
        newProjectie.GetComponent<Projectile>().Init(speed, -1.25f, 10, damage, agent.gameObject).SetBurn(true, 1 * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), 5);
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