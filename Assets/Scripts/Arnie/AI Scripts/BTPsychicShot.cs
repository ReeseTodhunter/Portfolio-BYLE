using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTPsychicShot : BTNode
{
    private GameObject projectilePrefab, muzzleFlashPrefab;
    private float cooldown;
    private float timeLastUsed = 0;
    private float projectileCount, currProjectileCount;
    private float spreadInDegrees;
    private float damage, velocity;
    private string entryAnim, exitAnim;
    private float timeBetweenShot, shotTimer;

    private float waitDuration, waitTimer;

    private Transform spawnPos;
    private enum ScriptState
    {
        inactive,
        shooting,
        exit
    }
    private ScriptState currState;
    public BTPsychicShot(BTAgent _agent, GameObject _projectilePrefab, GameObject _muzzleFlashPrefab, Transform _spawnPos, float _waitDuration, float _cooldown, float _timeBetweenShot, int _projectileCount, int _spreadInDegrees, float _damagePerProjectile, float _velocity, string _entryAnim = "", string _exitAnim = "")
    {
        agent = _agent;
        projectilePrefab = _projectilePrefab;
        muzzleFlashPrefab = _muzzleFlashPrefab;
        spawnPos = _spawnPos;
        cooldown = _cooldown;
        timeBetweenShot = _timeBetweenShot;
        projectileCount = _projectileCount;
        spreadInDegrees = _spreadInDegrees;
        damage = _damagePerProjectile;
        velocity = _velocity;
        entryAnim = _entryAnim;
        exitAnim = _exitAnim;
        waitDuration = _waitDuration;
    }
    public override NodeState Evaluate(BTAgent agent)
    {
        if (Time.time - timeLastUsed < cooldown) { return NodeState.FAILURE; }

        //////////////////////////
        if (EQS.instance.GetNearestNode(agent.transform.position).GetLineOfSight())
        {
            agent.SetCurrentAction(this);
            currState = ScriptState.shooting;
            agent.GetComponent<Animator>().Play(entryAnim);
            currProjectileCount = 0;
            shotTimer = 0;
            return NodeState.SUCCESS;
        }
        else
        {

            return NodeState.FAILURE;
        }

    }
    public override void UpdateNode(BTAgent agent)
    {
        switch (currState)
        {
            case ScriptState.inactive:
                break;
            case ScriptState.shooting:
                waitTimer += Time.deltaTime;
                if (waitTimer < waitDuration) { break; }
                agent.SetMovementEnabled(false);
                if (currProjectileCount >= projectileCount) { currState = ScriptState.exit; }
                shotTimer += Time.deltaTime;
                if (shotTimer <= timeBetweenShot) { break; }
                shotTimer = 0;
                currProjectileCount++;
                GameObject projectile = GameObject.Instantiate(projectilePrefab, spawnPos.position, spawnPos.rotation);
                projectile.transform.rotation = spawnPos.rotation;
                Vector3 angle = projectile.transform.eulerAngles;
                angle.y += Random.Range(-spreadInDegrees / 2, spreadInDegrees / 2);
                projectile.transform.eulerAngles = angle;
                projectile.transform.position = new Vector3(projectile.transform.position.x, 0.7f, projectile.transform.position.z);
                projectile.GetComponent<Projectile>().Init(velocity, 0, 10, damage, agent.gameObject);
                //shoot
                break;
            case ScriptState.exit:
                //exit
                agent.SetMovementEnabled(true);
                waitTimer = 0;
                ExitNode();
                break;
        }
    }
    private void ExitNode()
    {
        agent.ClearCurrentAction();
        currState = ScriptState.inactive;
        agent.GetComponent<Animator>().Play(exitAnim);
        timeLastUsed = Time.time;
    }
}