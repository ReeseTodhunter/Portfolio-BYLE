using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTShopKeeperBeam : BTNode
{
    private float windupDuration;
    private GameObject LaserObject;
    private Vector3 startPos, endPos, currPos;
    private float radius, damagePerSecond, duration;
    private float timer = 0;
    private enum ScriptState
    {
        inactive,
        charge,
        chargeWait,
        beam,
        exit
    }
    private ScriptState currState = ScriptState.inactive;
    private Material laserMat;
    private float tickTimer = 0;
    private float ticksPerSecond;
    private LineRenderer lineRnd;
    private Quaternion lockedRot;
    private float moveDuration = .5f, moveTimer = 0;
    public BTShopKeeperBeam(BTAgent _agent, GameObject _prefab, float _windUpDuration, float _radius, float _damagePerSecond, float _duration, Material _laserMat, float _ticksPerSecond = 5)
    {
        agent = _agent;
        LaserObject = GameObject.Instantiate(_prefab, agent.transform.position, Quaternion.identity);
        lineRnd = LaserObject.GetComponent<LineRenderer>();
        windupDuration = _windUpDuration;
        radius = _radius;
        damagePerSecond = _damagePerSecond;
        duration = _duration;
        laserMat = _laserMat;
        ticksPerSecond = _ticksPerSecond;
    }
    public override NodeState Evaluate(BTAgent agent)
    {
        currState = ScriptState.charge;
        timer = 0;
        agent.SetCurrentAction(this);
        agent.SetMovementEnabled(false);
        LaserObject.SetActive(true);
        moveTimer = 0;
        currPos = agent.projectileSpawn.position;
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent agent)
    {
        LaserObject.transform.position = agent.projectileSpawn.position;
        LaserObject.transform.forward = agent.projectileSpawn.forward;
        lineRnd.SetPosition(0, LaserObject.transform.position);
        lineRnd.SetPosition(1, currPos);
        endPos = FindEndLocation();
        LaserObject.transform.GetChild(2).position = lineRnd.GetPosition(1);
        switch (currState)
        {
            case ScriptState.inactive:
                break;
            case ScriptState.charge:

                agent.GetComponent<Animator>().speed = 1 / windupDuration;
                //agent.GetComponent<Animator>().Play("ShopKeeperBeamCharge");
                LaserObject.GetComponent<Animator>().speed = 1 / windupDuration;
                LaserObject.GetComponent<Animator>().Play("Windup", 0);

                currState = ScriptState.chargeWait;
                timer = 0;
                break;
            case ScriptState.chargeWait:
                //Wait for beam attack
                ChargeWaitUpdate();
                break;
            case ScriptState.beam:
                BeamUpdate();
                break;
            case ScriptState.exit:
                agent.ClearCurrentAction();
                agent.SetLookingAtPlayer(true);
                agent.SetMovementEnabled(true);
                LaserObject.GetComponent<Animator>().Play("WindDown", 0);
                //agent.GetComponent<Animator>().Play("ShopKeeperLaserShoot");
                break;
        }
        return;
    }

    private void ChargeWaitUpdate()
    {
        timer += Time.deltaTime;
        if (timer < windupDuration) { return; }
        timer = 0;
        tickTimer = 99;
        agent.GetComponent<Animator>().speed = 1;
        //agent.GetComponent<Animator>().Play("ShopKeeperLaserShoot");
        LaserObject.GetComponent<Animator>().Play("Active");
        lockedRot = agent.transform.rotation;
        agent.SetLookingAtPlayer(false);
        currState = ScriptState.beam;
    }
    private void BeamUpdate()
    {
        timer += Time.deltaTime;
        tickTimer += Time.deltaTime;
        moveTimer += Time.deltaTime;
        startPos = agent.projectileSpawn.position;
        currPos = Vector3.Lerp(startPos, endPos, moveTimer / moveDuration);

        agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, Quaternion.LookRotation((PlayerController.instance.transform.position - agent.transform.position).normalized), Time.deltaTime * 7.5f);

        float distance = Vector3.Distance(currPos, startPos);
        if (tickTimer > 1 / ticksPerSecond)
        {
            tickTimer = 0;
            RaycastHit hit;
            if (Physics.Raycast(agent.projectileSpawn.position, agent.transform.forward, out hit, distance, LayerMask.GetMask("Untraversable", "Player", "Hurtbox", "SeeThrough"), QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject == PlayerController.instance.gameObject)
                {
                    float damage = damagePerSecond / ticksPerSecond;
                    PlayerController.instance.Damage(damage, false, false);
                }
            }
        }
        if (timer < duration) { return; }
        timer = 0;
        currState = ScriptState.exit;
    }
    public Vector3 FindEndLocation()
    {
        RaycastHit hit;
        Vector3 direction = agent.transform.forward;
        direction.Normalize();
        Vector3 endLocation;
        if (Physics.Raycast(agent.projectileSpawn.position, direction, out hit, 50, LayerMask.GetMask("Untraversable", "Player", "Hurtbox", "SeeThrough"), QueryTriggerInteraction.Ignore))
        {
            endLocation = hit.point;
            endLocation.y = 1;
        }
        else
        {
            endLocation = agent.transform.position + direction * 50;
        }
        return endLocation;
    }
    public override void DeInitialiseNode(BTAgent agent)
    {
        if (LaserObject != null) { GameObject.Destroy(LaserObject); }
    }
}
