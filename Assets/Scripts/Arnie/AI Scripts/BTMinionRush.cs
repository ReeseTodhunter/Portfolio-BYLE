using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTMinionRush : BTNode
{

    private float speed = 1;
    private float maxPounceDistance = 2;
    private float windUpDuration = 0.5f, windUpTimer = 0;
    private float pounceSpeed = 0;
    private float duration = 2, durationTimer = 0;
    private float overshootDistance = 0;
    private float damage = 0;
    private Vector3 startPos, endPos;
    private float hitRange = 0.75f;
    private bool leaveTrail = false, buildupParticles = false;
    private GameObject trail, buildup;
    ///<Summary>
    /// Setup Pounce Behaviour
    /// </Summary>
    /// <param name ="_maxPounceDistance">Max distance the attack will cover.</param>
    /// <param name ="_windUpDuration">How long in seconds is waited before peforming the attack</param>
    /// <param name ="_pounceSpeed">The Speed of the enemy during the pounce attack</param>
    private enum actionState { windup, calculateTarget, dash, grab, chokeslam };
    private actionState currentState = actionState.windup;
    private float chokeslamDuration = 1.25f;
    private float chokeslamTimer = 0;
    public BTMinionRush(float _maxPounceDistance, float _windUpDuration, float _pounceSpeed, float _overshootDistance = 10, float _damage = 25, float _hitRange = 1.5f, bool _leaveTrail = false, bool _buildUpParticles = false)
    {
        maxPounceDistance = _maxPounceDistance;
        windUpDuration = _windUpDuration;
        pounceSpeed = _pounceSpeed;
        overshootDistance = _overshootDistance;
        damage = _damage;
        hitRange = _hitRange;
        leaveTrail = _leaveTrail;
        buildupParticles = _buildUpParticles;
    }
    public override NodeState Evaluate(BTAgent _agent)
    {
        agent = _agent;
        currentState = actionState.windup;
        windUpTimer = 0;
        durationTimer = 0;
        chokeslamTimer = 0;
        EQSNode nearestNode = EQS.instance.GetNearestNode(agent.transform.position);
        if (nearestNode.GetLineOfSight() && nearestNode.GetDistance() < maxPounceDistance)
        {
            agent.SetCurrentAction(this);
            agent.SetMovementEnabled(false);
            if (leaveTrail)
            {
                if (trail == null) { CreateTrail(); }
                trail.GetComponent<ParticleSystem>().Stop();
            }
            if (buildupParticles)
            {
                if (buildup == null) { CreateBuildup(); }
                buildup.GetComponent<ParticleSystem>().Play();
            }
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
    public override void UpdateNode(BTAgent agent)
    {
        switch (currentState)
        {
            case actionState.windup:
                windUpTimer += Time.deltaTime;
                if (windUpTimer >= windUpDuration) { currentState = actionState.calculateTarget; }
                break;
            case actionState.calculateTarget:
                if (tryGetEndPos())
                {
                    buildup.GetComponent<ParticleSystem>().Stop();
                    currentState = actionState.dash;
                    if (leaveTrail) { trail.GetComponent<ParticleSystem>().Play(); }
                    break;
                }
                else
                {
                    ExitNode(agent);
                    break;
                }
            case actionState.dash:
                agent.GetComponent<Animator>().Play("MinionDash");
                durationTimer += Time.deltaTime;
                if (durationTimer > duration)
                {
                    ExitNode(agent);
                    break;
                }
                float distance = Vector3.Distance(agent.transform.position, endPos);
                if (distance < hitRange) { ExitNode(agent); return; }
                if (Vector3.Distance(agent.transform.position, PlayerController.instance.transform.position) < hitRange)
                {
                    if (PlayerController.instance.GetFreezeGameplayInput())
                    {
                        ExitNode(agent);
                        return;
                    }
                    /*currentState = actionState.grab;
                    PlayerController.instance.transform.parent = agent.projectileSpawn;
                    PlayerController.instance.transform.position = agent.projectileSpawn.position;
                    PlayerController.instance.FreezeGameplayInput(true);
                    agent.SetLookingAtPlayer(false);*/
                    agent.SetVelocity(Vector3.zero);
                    //PlayerController.instance.GetComponent<Collider>().enabled = false;
                    AttackPlayer();
                    ExitNode(agent);
                    break;
                }
                Vector3 velocity = endPos - startPos;
                velocity.Normalize();
                velocity *= pounceSpeed;
                agent.SetVelocity(velocity);
                break;
            case actionState.grab:
                //Start chokeslam animation
                agent.GetComponent<Animator>().Play("Chokeslam");
                currentState = actionState.chokeslam;
                break;
            case actionState.chokeslam:
                chokeslamTimer += Time.deltaTime;
                if (chokeslamTimer >= chokeslamDuration)
                {
                    AttackPlayer();
                    PlayerController.instance.transform.parent = null;
                    PlayerController.instance.FreezeGameplayInput(false);
                    Vector3 pos = PlayerController.instance.transform.position;
                    pos.y = 1;
                    PlayerController.instance.transform.rotation = Quaternion.identity;
                    PlayerController.instance.transform.position = pos;
                    PlayerController.instance.GetComponent<Collider>().enabled = true;
                    agent.SetLookingAtPlayer(true);
                    ExitNode(agent);
                }
                break;
        }
    }
    private bool tryGetEndPos()
    {
        startPos = agent.transform.position;
        Vector3 directionOfPounce = PlayerController.instance.transform.position - agent.transform.position;
        directionOfPounce.Normalize();
        RaycastHit hit;
        endPos = PlayerController.instance.transform.position + directionOfPounce * overshootDistance;
        if (Physics.SphereCast(agent.transform.position, .5f, directionOfPounce, out hit, maxPounceDistance, 1 << 8))
        {
            if (Vector3.Distance(hit.point, agent.transform.position) < 5) { return false; }
            endPos = hit.point - directionOfPounce * 3f;
            endPos.y = agent.transform.position.y;
        }
        return true;
    }
    private void AttackPlayer()
    {
        PlayerController.instance.Damage(damage, false, true);
    }
    private void ExitNode(BTAgent agent)
    {
        if (trail != null) { trail.GetComponent<ParticleSystem>().Stop(); }
        if (buildup != null) { buildup.GetComponent<ParticleSystem>().Stop(); }
        agent.ClearCurrentAction();
        agent.SetMovementEnabled(true);
    }
    #region Trail
    private void CreateTrail()
    {
        if (trail != null)
        {
            DestroyTrail();
        }
        var trailPrefab = Resources.Load("VFX/MinionDashTrail");
        if (trailPrefab == null)
        {
            throw new System.IO.FileNotFoundException("Failed to find dash prefab, please check resource folder");
        }
        trail = GameObject.Instantiate(trailPrefab as GameObject, agent.transform.position + agent.transform.forward * -.5f, agent.transform.rotation);
        trail.transform.parent = agent.transform;
    }
    private void DestroyTrail()
    {
        if (trail == null) { return; }
        GameObject.Destroy(trail.gameObject);
    }
    #endregion
    #region BuildUp
    private void CreateBuildup()
    {
        if (trail != null)
        {
            DestroyBuildup();
        }
        var trailPrefab = Resources.Load("VFX/MinionDashBuildup");
        if (trailPrefab == null)
        {
            throw new System.IO.FileNotFoundException("Failed to find dash prefab, please check resource folder");
        }
        Vector3 offset = new Vector3(0, -0.9f, -0.75f); //trust me bro
        buildup = GameObject.Instantiate(trailPrefab as GameObject, agent.transform.position, agent.transform.rotation);
        buildup.transform.parent = agent.transform;
        buildup.transform.localPosition = offset;
    }
    private void DestroyBuildup()
    {
        if (buildup == null) { return; }
        GameObject.Destroy(buildup.gameObject);
    }
    #endregion

    public override void DeInitialiseNode(BTAgent agent)
    {
        DestroyTrail();
        DestroyBuildup();
    }
}
