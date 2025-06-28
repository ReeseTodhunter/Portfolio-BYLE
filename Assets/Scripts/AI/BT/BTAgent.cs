using System.Collections;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class BTAgent : Character
{
    /*
        Behaviour Tree AI agent. This script uses a Behaviour Tree to 
        evaluate what action it should perform. This AI should be able to
        take in any tree / brain and be able to perform it without issue.

        v23w03
        Complete BTAgent overhaul into a twin behaviour tree system. Gameplay 
        Actions and movement are seperated into different trees. Action tree
        can disable the movement tree when required. 
        
        -Tom
    */

    [Header("============================================================")]
    [Header("Behaviour Tree Settings")] //refresh rate for the two behaviour trees
    public Tree ActionTree;
    public float actionsRefreshRate = 2;
    public Tree MovementTree;
    public float movementRefreshRate = 2;

    [Space(6)]
    [Header("============================================================")]
    [Header("UI Settings")] //Settings for the display name (if boss) and the healthbar reference
    public string DisplayName = "???";
    public EnemyHealthBar healthBar;

    [Space(6)]
    [Header("============================================================")]
    [Header("Coin Drops")] //Drop stats, the coin prefab, how many coins drop on death and its chance
    public GameObject coinPrefab;
    [SerializeField] float coinsOnDeath = 1;
    [Range(0,1)] public float coinsDropChance = 0.25f;

    [Space(6)]
    [Header("============================================================")]
    [Header("Transition Settings")] //settings for spawning and dying
    [SerializeField] private bool instantSpawn = false; //Skips spawn animation
    [SerializeField] private  float deathDuration = 2; //How long the agent ragdolls on death for
    [SerializeField] private  float dissapearDuration = 2; //How long the agent sinks into the floor for
    [SerializeField] private float dissapearSpeed = 2; //How fast the the agent sinks
    [Space(6)]
    [Header("============================================================")]
    [Header("Effect Settings")]
    public GameObject SpawnEffectPrefab; //Spawn effect
    public GameObject eliteEffect; //Elite effect
    public GameObject model; //Model for the agent
    public bool dismembersOnDeath = false; //Whether the agent get dismembered on death
    public List<GameObject> dismemberObjects = new List<GameObject>(); //list of dismemberable body parts
    [Range(0,1)]
    public float chancePerDismember = 0.5f; //Chance per dismember objects to dismember
    [Space(6)]
    [Header("============================================================")]
    [Header("Elite Settings")]
    public bool isElite = false; //Is the agent elite
    public List<Transform> eliteEyes; //the list of transforms that elite fire comes out of 
    [Space(6)]
    [Header("============================================================")]
    [Header("Misc Settings")]
    public Transform projectileSpawn; //Where the agents projectile spawn from
    public bool showGizmos = false; //Show agent gizmos
    public List<BTModifier> modifiers = new List<BTModifier>(); //list of modifiers for the agent

    private float actionTimer = 99, movementTimer = 99; //Enemies come out swinging
    private bool movementTreeUnlocked = true, actionTreeUnlocked = true; //Whether each tree is enabled or not
    private List<Vector3> gizmoPath; //pathfinding path
    private Vector3 gizmoTargetPos; //target for pathfinding
    private bool isActive = false; //whether the agent is active or not
    private Vector3 movementVelocity = Vector3.zero; //velocity of the agent
    private float stunTimer = 0, stunDuration = 0; //stun variables
    public float maxOvershield, currOvershield; //overshield
    public float lastDmgTaken; //last damage taken
    private bool lookAtPlayer = true; //whether the agent looks at the player
    private float delayTimer = 0; //delay timer
    private enum AgentState //state of the agent
    {
        SPAWNING,
        ACTIVE,
        DYING,
        DISAPPEAR
    }
    private enum SpawnType //Type of agent spawn
    {
        EnemySpawner,
        AgentMinion,
        BossSpawn
    }
    public enum EnemyType //Type of agent
    {
        standard,
        Elite,
        Boss
    }
    public EnemyType enemyType; //current enemy type
    private MinionsModifier spawner = null; //spawner
    private BossSpawner bossSpawner = null;
    private SpawnType agentType = SpawnType.EnemySpawner; //current spawn type
    private AgentState currState = AgentState.SPAWNING; //current agent state
    private float deathTimer = 0, dissapearTimer = 0; //death settings
    private float noAttackDuration = 1,noAttackTimer = 0; //no attack settings
    private bool canAttack = false; //whether the agent can attack (prevents agent from attacking when they spawn)
    
    void Start()
    {
        //Physics.IgnoreLayerCollision(10,10,true); //ignore collision for projectiles
        delayTimer = 0;
        deathTimer = 0;
        dissapearTimer = 0;
        CharacterAwake(); //initialise character settings
        if(ActionTree != null){ActionTree.InitialiseTree(this);} //Init action tree
        if(MovementTree != null){MovementTree.InitialiseTree(this);}//Init movement tree
        isActive = true;
        SetMovementEnabled(true); //Enable movement tree
        SetActionsEnabled(true); //enable action tree
        currState = AgentState.SPAWNING;
        Vector3 pos = transform.position;
        foreach(BTModifier modifier in modifiers){modifier.Initialise(this);} //init all modifiers
        if(instantSpawn) //If instant spawn skip spawn effect
        {
            model.SetActive(true);
            currState = AgentState.ACTIVE;
            canAttack = true;
            foreach(BTModifier modifier in modifiers){modifier.Initialise(this);}
            SetSpawningFinished();
            if(Time.time < 1){delayTimer = 0.5f;}
            return;
        }
        GameObject spawnEffect = Instantiate(SpawnEffectPrefab, pos, Quaternion.identity); //Instantiate spawn effect
        spawnEffect.GetComponent<SpawnEffect>().Initialise(this); //init spawn effect
        model.SetActive(false); //hide agent
        canAttack = false;
    }
    void Update()
    {
        switch (currState)
        {
            case AgentState.SPAWNING: //wait for spawn to finish
                break;
            case AgentState.ACTIVE: //behave normally
                if(GameManager.GMinstance == null)
                {
                    ActiveUpdate();
                    break;
                }
                else if (GameManager.GMinstance.currentState == GameManager.gameState.playing)
                {
                    ActiveUpdate();
                }
                break;
            case AgentState.DYING: //ragdoll on death
                DyingUpdate();
                break;
            case AgentState.DISAPPEAR: //dissapear into the ground
                DissapearUpdate();
                break;
        }

    }
    void ActiveUpdate()
    {
        delayTimer += Time.deltaTime;
        if(delayTimer < 0.5f){return;}
        //Guard clause in case agent disabled
        if(!isActive){return;}
        if (noAttackTimer < noAttackDuration)
        {
            noAttackTimer += Time.deltaTime;
        }
        else
        {
            canAttack = true;
        }
        CharacterUpdate(); //update character settings

        if(isStunned && enemyType == EnemyType.standard) //if is stunned return
        {
            return;
        }
        //Look at player
        if(lookAtPlayer)
        {
            Vector3 playerPos = PlayerController.instance.transform.position;
            playerPos.y = transform.position.y;
            transform.LookAt(playerPos);
        }

        //Update timers
        actionTimer += Time.deltaTime;
        movementTimer += Time.deltaTime;
        //Update Trees
        if(movementTreeUnlocked && MovementTree != null)
        {
            if(MovementTree.hasRunningNode)
            {
                MovementTree.UpdateRunningNode();
            }
            else if(movementTimer >= movementRefreshRate)
            {
                MovementTree.UpdateTree();
                movementTimer = 0;
            }
        }
        if(actionTreeUnlocked && ActionTree != null && canAttack)
        {
            if(ActionTree.hasRunningNode)
            {
                ActionTree.UpdateRunningNode();
            }
            else if(actionTimer >= actionsRefreshRate)
            {
                ActionTree.UpdateTree();
                actionTimer = 0;
            }
        }

        //On Update modifiers
        if(modifiers.Count > 0)
        {
            foreach(BTModifier modifier in modifiers)
            {
                if(modifier.type == BTModifier.modifierType.onUpdate)
                {
                    modifier.ActivateModifier(this);
                }
            }
        }
        
        //Update velocity
        this.GetComponent<Rigidbody>().velocity = movementVelocity * speed;
    }
    void DyingUpdate() //On death, ragdoll
    {
        deathTimer += Time.deltaTime;
        if(deathTimer >= deathDuration) //if death duration met, starting sinking into the ground
        {
            foreach(BTModifier modifier in modifiers)
            {
                if(modifier.type == BTModifier.modifierType.onDissapear){modifier.ActivateModifier(this);}
            }
            currState = AgentState.DISAPPEAR;
            Rigidbody rb = this.GetComponent<Rigidbody>(); //disable rigidbody and collider
            rb.isKinematic = true;
            rb.detectCollisions = false;
            this.GetComponent<Collider>().enabled = false;
        }
    }
    void DissapearUpdate() //after death ragdoll, sink into the ground
    {
        dissapearTimer += Time.deltaTime;
        if(dissapearTimer >= dissapearDuration) //after dissapear duration, destroy gameobject
        {
            Destroy(this.gameObject);
        }
        transform.position -= Vector3.up * Time.deltaTime * dissapearSpeed;
    }
    public override void OnHeal(float _heal) //Heal agent
    {
        if(enemyType != EnemyType.standard) { BossBar.instance.UpdateBar(this); }

        if(healthBar != null)
        {
            healthBar.ApplyDamage();
        }
        
    }
    public override void Damage(float dmg, bool ignoreImmunity = false, bool grantImmunity = true, bool ignoreResistance = false, EffectType type = EffectType.None, bool _isCrit = false) //Damage agent
    {
        if(currOvershield <= 0) //if no overshield, damage health
        {
            base.Damage(dmg, ignoreImmunity, grantImmunity, ignoreResistance,type, _isCrit);
            lastDmgTaken = dmg;
        }
        else //Else, damage overshield
        {
            currOvershield -= dmg;
            healthBar.ApplyDamage();
            if(dmg == 0) { return; }
            GameObject number = ProjectileLibrary.instance.GetProjectile(Projectiles.FLOATING_TEXT);
            GameObject instance = Instantiate(number, transform.position + Vector3.up, Quaternion.identity);
            instance.GetComponent<ItemPickupFade>().SetText(dmg.ToString(),type,_isCrit);
        }

    }
    protected override void OnDamage(float dmg, bool ignoreImmunity = false, bool grantImmunity = true, bool ignoreResistance = false, EffectType type = EffectType.None, bool _isCrit = false) //When agent damaged
    {
       
        if(currState != AgentState.ACTIVE){return;}
        foreach(BTModifier modifier in modifiers) //Update all on damage modifiers
        {
            if(modifier.type == BTModifier.modifierType.onDamage){modifier.ActivateModifier(this);}
        }
        if(TryGetComponent<DamageFlash>(out DamageFlash flash)) //activate damage flash
        {
            switch(type)
            {
                case EffectType.Burn:
                    flash.StartFlash(.25f,true,DamageFlash.FlashType.BURN);
                    break;
                default:
                    flash.StartFlash(.25f,true,DamageFlash.FlashType.DAMAGE);
                    break;
            }
        }   
        else //Debug error if no damage flash is attached to the agent
        {
            Debug.LogWarning("Error : Agent of type '" + DisplayName + "' has no damage flash script attached");
        }
        if(healthBar != null){ healthBar.ApplyDamage(); } //apply damage to healtbars
        if(enemyType != EnemyType.standard) { BossBar.instance.UpdateBar(this);} //update boss bar if needed
        //Spawn damage number
    }
    protected override void Die()
    {
        // Put enemy to bullet layer so that the players bullets cant collide with it
        gameObject.layer = LayerMask.NameToLayer("Bile");
        gameObject.tag = "Untagged";


        if (currState == AgentState.DYING || currState == AgentState.DISAPPEAR) //return if already dying
        {
            return;
        }

        if (isElite) //achievements
        {
            //VIP Slayer
            AchievementSystem.UnlockAchievement(5);
            AchievementSystem.Init();
            SaveLoadSystem.instance.SaveAchievements();
            GameManager.GMinstance.UnlockSteamAchievement("VIP_Slayer");
        }

        if (ActionTree != null) { ActionTree.DeInitialiseTree(); } //stop trees
        if (MovementTree != null) { MovementTree.DeInitialiseTree(); }
        if(enemyType == EnemyType.standard) { healthBar.gameObject.SetActive(false); } //hide healthbars

        PlayerController.instance.score += score; //add score
        if (GameManager.GMinstance != null) //iterate enemies killed
        {
            Debug.Log(characterID);
            GameManager.GMinstance.enemiesEncountered[characterID] = true;
            GameManager.GMinstance.enemiesKilled += 1;
            GameManager.GMinstance.IncrementSteamStatsForAchievements("Kills", 1);

        }


        
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None; //apply ragdoll force to agent
        Vector3 force = transform.position - PlayerController.instance.transform.position;
        force.y = 0;
        force.Normalize();
        this.GetComponent<Rigidbody>().velocity = force * 20;
        currState = AgentState.DYING;

        SetGameLayerRecursive(gameObject, 0); //set layer of enemy to 0 so there's not artifacting
        foreach(BTModifier modifier in modifiers) //activate all relevant modifiers
        {
            if(modifier.type == BTModifier.modifierType.onDeath){modifier.ActivateModifier(this);}
        }
        float chance = Random.Range(0.0f, 1.0f);
        if(chance < coinsDropChance && coinPrefab != null) //drop coins
        {
            Vector3 rnd = Vector3.zero;
            // Triple coins dropped if player has the powerup and gets lucky
            float totalCoins = coinsOnDeath;
            if (PlayerController.instance.GetModifier(ModifierType.ExtraCoinChance) > Random.Range(0.0f, 1.0f)) totalCoins *= 3;

            for(int i = 0; i < totalCoins; i++)
            {
                float x = Random.Range(-1f,1f);
                float z = Random.Range(-1f,1f);
                rnd = new Vector3(x,1,z);
                Instantiate(coinPrefab,transform.position + rnd,Quaternion.identity);
            }
        }
        transform.SetParent(null);

        if (EnemySpawningSystem.instance != null) //unsub enemy from spawner
        {
            EnemySpawningSystem.instance.UnsubscribeEnemy(this);
        }

        if (enemyType != EnemyType.standard) //disconnect from boss bar if needed
        {
            BossBar.instance.UnBindAgentToBossBar(this);
        }
        //Set dismember
        if (!dismembersOnDeath) { return; }
        foreach (GameObject obj in dismemberObjects) //Dismember parts of the agent
        {
            float dismemberChance = Random.Range(1, 10);
            if (dismemberChance > chancePerDismember * 10) { continue; }

            // Do this so the collider doesnt collide with bullets
            obj.layer = LayerMask.NameToLayer("Bile");
            obj.gameObject.tag = "Player";

            obj.transform.parent = null;
            obj.AddComponent<KillScript>();
            obj.GetComponent<KillScript>().lifeTime = 5;
            obj.AddComponent<BoxCollider>();
            obj.AddComponent<Rigidbody>();
            obj.GetComponent<Rigidbody>().velocity = (transform.position - obj.transform.position).normalized * 10;
        }

        // EXPLODE!!! if the player picked up the powerup :�)
        if (PlayerController.instance.GetModifier(ModifierType.EnemyExplosion) > Random.Range(0.0f, 1.0f))
        {
            Vector3 explosionPos = transform.position;
            explosionPos.y = 0.5f;
            float explosionRadius = 5.0f;
            Instantiate(ProjectileLibrary.instance.GetProjectile(Projectiles.BYLE_EXPLOSION), explosionPos, Quaternion.identity);
            Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.TryGetComponent<BTAgent>(out BTAgent agent) && gameObject != collider.gameObject)
                {
                    // 10 damage with falloff
                    agent.Damage(10.0f * (1 - (Vector3.Distance(transform.position, collider.transform.position)/explosionRadius)));
                }
            }
            CameraController.instance.ShakeCameraOverTime(0.5f, 1.5f);
        }

        
    }
    public override void Stun(float time)
    {
        base.Stun(time);
    }
    protected override void OnStunEnd()
    {

    }
    void OnDestroy() //Unsubscribe the agent from relevant spawning system
    {
        switch (agentType)
        {
            case SpawnType.EnemySpawner:
                if(EnemySpawningSystem.instance == null)
                {
                    break;
                }
                EnemySpawningSystem.instance.UnsubscribeEnemy(this);

                break;
            case SpawnType.AgentMinion:
                spawner.UnsubscribeMinion(this);
                break;
            case SpawnType.BossSpawn:
                bossSpawner.UnSubscribeBoss(this);
                break;
            default:
                break;
        }
    }
    //Get any relevant variables
    #region Getters
    public float GetSpeed(){return speed;}
    public bool isAlive()
    {
        if(currState == AgentState.SPAWNING || currState == AgentState.ACTIVE)
        {
            return true;
        }
        return false;
    }
    #endregion
    //Set any relevant variables
    #region Setters
    public void AddBTModifier(BTModifier _modifier)
    {
        if(modifiers.Contains(_modifier)){return;}
        modifiers.Add(_modifier);
        _modifier.Initialise(this);
    }

    public void SetInstantSpawn(bool isInstant)
    {
        instantSpawn = isInstant;
    }
    public void SetLookingAtPlayer(bool _look)
    {
        lookAtPlayer = _look;
    }
    public void SubscribeAgent(MinionsModifier _minionSpawner)
    {
        agentType = SpawnType.AgentMinion;
        _minionSpawner.SubscribeMinion(this);
        spawner = _minionSpawner;
    }
    public void SubscribeAgent(EnemySpawningSystem enemySpawner)
    {
        agentType = SpawnType.EnemySpawner;
        enemySpawner.SubscribeEnemy(this);
    }
    public void SubscribeAgent(BossSpawner _bossSpawner)
    {
        agentType = SpawnType.BossSpawn;
        bossSpawner = _bossSpawner;
    }
    public void SetDeathDuration(float _duration)
    {
        deathDuration = _duration;
    }
    public void SetSpawningFinished()
    {
        if(currState != AgentState.SPAWNING){return;}
        currState = AgentState.ACTIVE;
        CharacterAwake();
        if(model != null){model.SetActive(true);}
        if (this.TryGetComponent<DamageFlash>(out DamageFlash flash))
        {
            flash.StartFlash(1f, true, DamageFlash.FlashType.SPAWNING);
        }
        if (enemyType != EnemyType.standard)
        {
            BossBar.instance.BindAgentToBossBar(this);
        }
    }
    public void SetMaxHealth(float _maxHealth){ baseMaxHealth = _maxHealth; maxHealth = _maxHealth; health = _maxHealth; }
    ////Set Gizmos
    public void UpdateGizmos(List<Vector3> path)
    {
        gizmoPath = path;
    }
    public void UpdateGizmos(Vector3 targetPos)
    {
        gizmoTargetPos = targetPos;
    }
    ////

    ////Allows trees to be toggled
    public void SetMovementTreeLockState(bool isUnlocked)
    {
        movementTreeUnlocked = isUnlocked;
    }
    public void SetActionTreeLockState(bool isUnlocked)
    {
        actionTreeUnlocked = isUnlocked;
    }
    ////

    ////Locks trees into certain behaviour (probably isn't necessary)
    public void SetCurrentMovement(BTNode activeMove)
    {
        if(activeMove == null){MovementTree.unlockActiveNode(); return;}
        MovementTree.lockActiveNode(activeMove);
    }
    public void ClearCurrentMovement(){SetCurrentMovement(null); movementVelocity = Vector3.zero;}
    public void SetCurrentAction(BTNode activeAction)
    {
        if(activeAction == null){ActionTree.unlockActiveNode(); return;}
        ActionTree.lockActiveNode(activeAction);
    }
    public void ClearCurrentAction(){SetCurrentAction(null);}
    
    //Unlocks or locks trees
    public void SetMovementEnabled(bool _isEnabled)
    {
        movementTreeUnlocked = _isEnabled;
        if(!_isEnabled)
        {
            movementVelocity = Vector3.zero;
        }
    }
    public void SetActionsEnabled(bool _isEnabled)
    {
        actionTreeUnlocked = _isEnabled;
    }
    public void SetVelocity(Vector3 _velocity)
    {
        movementVelocity = _velocity;
    }
    #endregion

    //BYLE
    public override void BYLEInteraction()
    {
        return;
    }

    private void SetGameLayerRecursive(GameObject _go, int _layer) //Ripped this from https://forum.unity.com/threads/help-with-layer-change-in-all-children.779147/, recursively gets every gameobject in a transforms child tree
    {
        _go.layer = _layer;
        foreach (Transform child in _go.transform)
        {
            child.gameObject.layer = _layer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, _layer);
            
        }
    }
    public void OnDrawGizmos() //Gizmos stuffs
    {
        if(!showGizmos)
        {
            return;
        }
        if(gizmoTargetPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(gizmoTargetPos, Vector3.one * 0.5f);
            Gizmos.DrawLine(transform.position, gizmoTargetPos);
        }
        if(gizmoPath != null)
        {
            foreach(Vector3 node in gizmoPath)
            {
                Gizmos.color = Color.white; 
                Gizmos.DrawWireCube(node,Vector3.one * 0.25f);
            }
        }
    }
}
