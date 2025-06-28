using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRevenantVortex : BTNode
{
    /*
     * This Node describes how the Revenant boss performs its vortex attack;
     * 1 - The boss moves to the centre of the room, defined as roomCentre transform
     * 2 - The boss performs its startup animation
     * 3 - The boss spawns two rings of projectiles. The first is near the boss and the second is 
     * further away, causing the player to be blocked inside the resulting ring
     * 4 - The boss spawns cover that blocks the player from shooting it, as well as a large
     * projectile that slowly travels around the two rings. 
     * This forces the player to move around the map to get behind the cover and shoot the boss
     * 5 - This behaviour will continue for a set amount of time or if the boss takes too much damage
     * 
     * -Tom
    */

    #region Variables
    private Transform roomCentre; //Where the boss wil stand
    private float duration = 5f; //Duration of behaviour
    private float cooldown, lastTimeUsed = 0;
    private float durationTimer = 0;
    private float startingHealth;
    private float maxDamageTaken = 40; //Damage applied to boss required to forcefully exit the behaviour
    private float innerRingRadius = 5, outerRingRadius = 20; //Radius of projectile rings
    private float innerRingCount, outerRingCount; //Amount of projectiles in each ring
    private Transform innerRingPivot, outerRingPivot, thidRingPivot, scythePivot; //Pivot of rings, allowing them to spin
    private GameObject rotatingScythe; //The projectile that travels around the two rings
    private GameObject scythePrefab, projectilePrefab; //Prefabs for projectiles
    private GameObject shieldPrefab, shield; //Prefabs for cover that the boss casts
    private List<GameObject> projectiles = new List<GameObject>();
    #endregion

    //Enum that controls the flow of the script
    private enum ScriptState
    {
        IDLE,
        MOVE_TO_CENTRE,
        WINDUP_ANIMATION,
        SPAWN_RINGS,
        SPIN_RINGS,
        EXIT
    }
    private ScriptState currState = ScriptState.IDLE;
    //Constructor
    public BTRevenantVortex(BTAgent _agent, float _cooldown, float _outerRingCount, float _innerRingCount, GameObject _scythePrefab, string _projectilePrefab, 
        GameObject _shieldPrefab, Transform _roomCentre, Transform _primaryRing, Transform _secondaryRing, Transform _tertiaryRing, Transform _scythePivot)
    {
        agent = _agent;
        cooldown = _cooldown;
        outerRingCount = _outerRingCount;
        innerRingCount = _innerRingCount;
        scythePrefab = _scythePrefab;
        projectilePrefab = Resources.Load(_projectilePrefab) as GameObject;
        shieldPrefab = _shieldPrefab;
        roomCentre = _roomCentre;
        innerRingPivot = _primaryRing;
        outerRingPivot = _secondaryRing;
        thidRingPivot = _tertiaryRing;
        scythePivot = _scythePivot;
    }
    //Evaluates whether the node is successful or not
    public override NodeState Evaluate(BTAgent agent)
    {
        if(Time.time - lastTimeUsed < cooldown) { return NodeState.FAILURE; }  //If node hasn't had time to cooldown, return
        if (roomCentre == null) { return NodeState.FAILURE; }
        DestroyProjectiles(); //Destroys any projectiles previously used in rings if somehow they're still around
        agent.SetCurrentAction(this); //Set the agents current action to this and disable movement
        agent.SetMovementEnabled(false);
        startingHealth = agent.GetHealth(); //Get current health of agent
        agent.GetComponent<Collider>().isTrigger = true; //Disable collider and rigidbody, so that the boss can glide to centre
        agent.GetComponent<Rigidbody>().useGravity = false;
        currState = ScriptState.MOVE_TO_CENTRE;
        agent.GetComponent<Animator>().speed = 1;
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent agent)
    {
        switch (currState) //Update the node
        {
            case ScriptState.IDLE: //Do nothing
                break;
            case ScriptState.MOVE_TO_CENTRE: //Move agent to centre of room
                Vector3 pos = agent.transform.position; //Get positions of agent and room centre
                pos.y = 0;
                Vector3 targetPos = roomCentre.transform.position;
                targetPos.y = 0;
                if(Vector3.Distance(pos,targetPos) < 2) //If within distance, move onto next state of node
                {
                    agent.GetComponent<Collider>().isTrigger = false;//re-enable collider and rigidibody
                    agent.GetComponent<Rigidbody>().useGravity = true;
                    currState = ScriptState.SPAWN_RINGS;  //move to next state
                    float y = agent.transform.position.y; //Snap to centre position
                    targetPos.y = y;
                    agent.transform.position = targetPos;
                    agent.SetVelocity(Vector3.zero); //Stop any velocity
                    agent.GetComponent<Animator>().Play("SpinWindup");
                    break; 
                }
                Vector3 velocity = targetPos - pos;//Move towards centre
                velocity.Normalize();
                velocity.y = 0;
                agent.SetVelocity(velocity * 7);
                break;
            case ScriptState.WINDUP_ANIMATION:
                durationTimer += Time.deltaTime;
                if(durationTimer < 2) { break; }
                currState = ScriptState.SPAWN_RINGS;
                break;
            case ScriptState.SPAWN_RINGS://spawn Rings
                agent.SetLookingAtPlayer(false); //stop agent from rotating
                innerRingPivot.transform.rotation = Quaternion.identity; //reset rotations
                outerRingPivot.transform.rotation = Quaternion.identity;
                Vector2 agentPos = new Vector2(agent.transform.position.x, agent.transform.position.z); //Get positions
                Vector2 playerPos = new Vector2(PlayerController.instance.transform.position.x, 
                    PlayerController.instance.transform.position.z);
                float radius = Vector2.Distance(agentPos, playerPos);
                SpawnRing(radius - 3.5f, 16, innerRingPivot); //Spawn rings
                SpawnRing(radius + 3.5f, 24, outerRingPivot);
                if(radius > 12)
                {
                    SpawnRing(radius - 7f, 12, thidRingPivot);
                }
                else
                {
                    SpawnRing(radius + 7f, 32, thidRingPivot);
                }

                Vector3 offset = agent.transform.position - PlayerController.instance.transform.position; //Get offset of player from agents pos
                Vector3 relativePos = agent.transform.position + offset; // add that offset to agent to get scythe pos
                relativePos.y = PlayerController.instance.transform.position.y; //fix y pos
                GameObject scythe = GameObject.Instantiate(projectilePrefab, relativePos, Quaternion.identity); //instantiate
                scythe.transform.parent = scythePivot; //set pivot
                scythe.GetComponent<Projectile>().Init(0, 0, 999, 0, agent.gameObject); //init projectile
                scythe.transform.localScale *= 3; //Make bigger
                projectiles.Add(scythe); //add to list
                scythe.GetComponent<Collider>().enabled = false; //Get rid of collider
                durationTimer = 0; //reset timer
                currState = ScriptState.SPIN_RINGS; //go to new 
                break;
            case ScriptState.SPIN_RINGS://spin rings
                innerRingPivot.transform.Rotate(Vector3.up * Time.deltaTime * 30);
                outerRingPivot.transform.Rotate(Vector3.up * Time.deltaTime * -30);
                thidRingPivot.transform.Rotate(Vector3.up * Time.deltaTime * -15f);
                scythePivot.transform.Rotate(Vector3.up * Time.deltaTime * -45f);
                if (CanHitPlayer())
                {
                    PlayerController.instance.Damage(5, false, true, false);
                }
                durationTimer += Time.deltaTime;
                {
                if(durationTimer >= duration || startingHealth - agent.GetHealth() >= maxDamageTaken)
                    currState = ScriptState.EXIT;
                    break;
                }
            case ScriptState.EXIT://exit 
                ExitNode();
                break;
        }
    }
    private void ExitNode()
    {
        lastTimeUsed = Time.time; //Reset cooldown
        agent.SetLookingAtPlayer(true); //Reset agent look
        DestroyProjectiles(); //Destroy all ring projectiles
        agent.ClearCurrentAction();
        agent.GetComponent<Animator>().Play("Idle");
        agent.SetMovementEnabled(true);
    }
    private void DestroyProjectiles()
    {
        if(projectiles.Count == 0) { return; }
        foreach (GameObject projectile in projectiles)
        {
            GameObject.Destroy(projectile);
        }
        projectiles.Clear();
        return;
    }
    private bool CanHitPlayer()
    {
        bool withinRange = false;
        foreach (GameObject proj in projectiles)
        {
            if (Vector3.Distance(proj.transform.position, PlayerController.instance.transform.position) < 1.25f * proj.transform.localScale.magnitude)
            {
                withinRange = true;
                break;
            }
        }
        if (!withinRange) { return false; }
        if (PlayerController.instance.IsInvulnerable()) { return false; }
        return true;
    }
    private void SpawnRing(float _radius, int _count, Transform _parent) //Spawn all the projectiles in a ring
    {
        Vector3 offset = Vector3.zero;
        Vector3 pos;
        for (int i = 0; i < _count; i++) //iterate for each projectile
        {
            offset = Quaternion.AngleAxis(360 * i / _count, Vector3.up) * Vector3.one; //Get the offset of current projectile using angle axis
            offset.y = 0; //remove y offset
            offset.Normalize();
            pos = agent.transform.position + offset * _radius; //Multiply by radius
            pos.y = 0.5f;
            GameObject newProjectile = GameObject.Instantiate(projectilePrefab, pos, Quaternion.identity); //instantiate projectile at pos
            newProjectile.GetComponent<Projectile>().Init(0, 0, 999, 0, agent.gameObject);
            projectiles.Add(newProjectile); //add to list
            newProjectile.GetComponent<Collider>().enabled = false;
            newProjectile.transform.parent = _parent; //Add to ring parent so it can spin
        }
        return;
    }
}
