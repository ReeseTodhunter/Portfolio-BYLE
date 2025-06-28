using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : Character
{
    HighScoreDisplay HighScoreDisplay;

    // General
    public static PlayerController instance;
    Rigidbody rb;
    private Animator animator;
    private int coins = 0;

    //Audio
    public List<AudioClip> damageAudio = null;    //Audio for taking a hit
    public List<AudioClip> pickupAudio = null;    //Item get sound effect
    public List<AudioClip> abilitySounds = null;

    // Movement
    bool freezeGameplayInputs = false; // Set to true to stop player from
    Vector3 previousMousePos;
    public float baseMovementControl = 15.0f; // How quickly the player will talk towards their current direction
    public float lesserMovementControl = 3.0f; // How quickly the player will talk towards their current direction when moving faster than usual

    // Dodge Roll
    public GameObject rollIndicator;
    public float rollSpeed; // How fast the player will roll
    public float rollLength; // How long a roll will last in seconds
    public float rollCooldownLength; // Time between each roll in seconds
    float rollTimer;
    float rollCooldownTimer;
    int rollCharges; // Amount of rolls the player currently has
    Vector3 rollDirection; // Direction of the roll

    // Active Ability
    public BaseActivePowerup currentAbility;
    float abilityTimer = 0.0f;

    // Weapon Fire
    public BaseWeapon primaryWeapon;
    public BaseWeapon secondaryWeapon;
    public Transform weaponParent;
    public Transform secondaryParent;
    public Transform modelCentre;
    bool canUseWeapons = true;
    float weaponSwapTime = 0.0f; // Used to stop firing weapons immediately after swapping
    private float scrollTimer = 0; // Used to stop the player from swapping weapons extremely fast

    //Weapon Specials
    public int NumOfChicks;

    //Misc
    private Vector3 directionOfMovement = Vector3.zero;
    public GameObject model;
    private Vector3 aimPos;
    private bool isRolling = false;

    // Character classes
    public enum SelectedCharacter
    {
        VLAD,
        MIKE,
        RAMBO,
        OL_ONE_EYE,
        DART,
        KARLOS,
        XERXES,
        KYLE
    }
    public SelectedCharacter currCharacter = SelectedCharacter.RAMBO;
    public Transform decor;

    // Achievement flags
    public bool playerHitInRoom = false;
    public bool playerHitInLevel = false;

    private void Awake()
    {
        // Instance for the enemy AIs
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this);
            }
        }
        instance = this;

        
        // Getting rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("You are stupid. Give the player a rigidbody.");
        }
        animator = this.GetComponent<Animator>();
        
        if(primaryWeapon != null)
        {
            primaryWeapon.GetComponent<Collider>().enabled = false;
            primaryWeapon.GetComponent<BaseWeapon>().currState = BaseWeapon.WeaponState.EQUIPPED;
            animator.Play(primaryWeapon.GetIdleAnimName());
        }
        if(secondaryWeapon != null)
        {
            secondaryWeapon.GetComponent<Collider>().enabled = false;
            secondaryWeapon.OnStow();
        }
        // Character awake code
        CharacterAwake();
        if(GameManager.GMinstance == null){return;}
        if (!GameManager.GMinstance.loadGame)
        {
            if (SceneManager.GetActiveScene().name == "BUILD")
            {
                SetPlayerCharacter();
            }
        }

    }
    private void Update()
    {
        CharacterUpdate(); // Inherited from character. Runs all character updates (status effects and other)
        if (!freezeGameplayInputs)
        {
            Movement(); // Runs the movement code for the player
            DodgeRoll(); // Checks if the player has dodge rolled and runs any dodge roll functionality
            ItemInteract(); // Checks if the player is picking up an item
            ActiveAbility(); // Activates any abilities the player uses

            // Weapons
            if (canUseWeapons && !isStunned)
            {
                UpdateWeapon(); // Attempts to fire weapons and any other functionality they may have
                SwitchWeapon(); // Attempts to switch weapons
            }
        }

        if (modelCentre != null) { ObstacleHidingScript.SetPlayerScreenPosition(transform); } // For showing player through objects
    }
    private void LateUpdate()
    {
        if(rollTimer > 0){return;} // If the player is rolling do not update aim
        if (!freezeGameplayInputs)
        {
            Aim(); // Updates the players current aim
        }
    }
    // Update Functions
    void Movement()
    {
        // ---------------------------------------------------------
        // MOVEMENT
        // ---------------------------------------------------------
        // If player is rolling then velocity changes are handled by dodge roll
        if (rollTimer <= 0.0f)
        {
            // Get player held direction
            Vector3 direction = GameManager.GMinstance.GetMovementVector3();
            direction.Normalize();

            // Get current speed and decide how much control player has over movement
            // Anything over 3 times regular speed is considered loss of control, this allows for knockback to actually be effective
            float movementControl = baseMovementControl;
            if (rb.velocity.magnitude > speed * 3f) 
            {
                movementControl = lesserMovementControl;
            }

            // Lerp between current velocity and new one based off directional input
            Vector3 newVelocity = Vector3.zero;
            newVelocity.x = Mathf.Lerp(rb.velocity.x, direction.x * speed, Time.unscaledDeltaTime * movementControl);
            newVelocity.z = Mathf.Lerp(rb.velocity.z, direction.z * speed, Time.unscaledDeltaTime * movementControl);

            rb.velocity = newVelocity;
        }
    }

    void DodgeRoll()
    {
        // ---------------------------------------------------------
        // DODGE ROLL
        // ---------------------------------------------------------
        // TO-DO:
        // � Better implementation for locking abilities during a roll

        // Start roll (User has pressed roll key and direction)
        if (GameManager.GMinstance.GetInputDown("keyRoll") && rollCharges > 0 && GameManager.GMinstance.GetMovementVector2() != Vector2.zero)
        {
            //Debug.Log("ROLL START!!!!");

            // Hide roll indicator if it is currently active
            rollIndicator.SetActive(false);

            // I-Frames
            Protect(rollLength);

            // Decrease roll charges
            rollCharges--;

            // Get roll direction
            rollDirection = GameManager.GMinstance.GetMovementVector3();
            rollDirection.Normalize();

            // Start roll timer
            rollTimer = rollLength; // Start roll timer
            rollCooldownTimer = rollCooldownLength; // Start roll cooldown timer (starts when rollTimer hits 0)

            // ANIMATION
            // Setting speed the of the roll animation based off time of the roll set in inspector
            animator.speed = 1 / rollLength;
            // Freeze all rigidbody rotation (stops animation freaking out when rolling into wall)
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            // Roll animation
            switch (GetMovementDirection())
            {
                case PlayerMoveDirection.Forwards:
                    animator.Play("RollForward");
                    break;
                case PlayerMoveDirection.Backwards:
                    animator.Play("RollBackward");
                    break;
                case PlayerMoveDirection.Left:
                    animator.Play("RollLeft");
                    break;
                case PlayerMoveDirection.Right:
                    animator.Play("RollRight");
                    break;
                default:
                    break;
            }

            // Fire bullet ring if player has said ability
            GameObject rollProjectile = ProjectileLibrary.instance.GetProjectile(Projectiles.GENERIC_PLAYER_BULLET);
            int rollBullets = (int)GetModifier(ModifierType.DodgeRollRing);
            if (rollBullets > 0 && rollProjectile != null)
            {
                float bulletAngle = 360.0f / rollBullets;
                for (int i = 0; i < rollBullets; ++i)
                {
                    Instantiate(rollProjectile, transform.position + Vector3.up, Quaternion.Euler(0, bulletAngle * i, 0)).GetComponent<Projectile>()
                        .Init(30.0f * (1.0f + GetModifier(ModifierType.ProjectileSpeed)), 0.0f, 3.0f, 1.0f * (1.0f + GetModifier(ModifierType.Damage)), this.gameObject)
                        .SetPierce((int)PlayerController.instance.GetModifier(ModifierType.Pierce), 1.0f)
                        .SetCrit(Random.Range(0,10) == 0, 1.25f);
                }
            }

            isRolling = true;
        }

        // During roll (Roll timer is counting down)
        if (rollTimer > 0.0f)
        {
            //transform.LookAt(transform.position - rollDirection); // Look in roll direction

            rb.velocity = rollDirection * rollSpeed * (1+GetModifier(ModifierType.Speed)) / Time.timeScale; // Move in roll direction

            rollTimer -= Time.unscaledDeltaTime; // Decrease roll timer
        }

        // End of the roll (Roll timer is finished and now cooldown timer is counting down)
        if (rollTimer <= 0.0f && rollCooldownTimer > 0.0f)
        {
            // Reset rigidbody constraints back to normal so player can walk properly into walls again
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            // Decrease roll cooldown timer
            rollCooldownTimer -= Time.unscaledDeltaTime * (1.0f + GetModifier(ModifierType.CooldownReduction));

            // Update player canvas UI
            rollIndicator.SetActive(true);
            rollIndicator.GetComponent<Slider>().value = GetRollCooldown();
            if(isRolling)
            {
                animator.speed = 1;
                isRolling = false;
                if(primaryWeapon != null)
                {
                    animator.Play(primaryWeapon.GetIdleAnimName());
                }
            }
            
        }

        // End of roll cooldown and missing a roll charge (The roll cooldown has finished)
        if (rollCooldownTimer <= 0.0f && rollCharges < 1+GetModifier(ModifierType.RollCharges))
        {
            rollCharges++;

            if (rollCharges < 1 + GetModifier(ModifierType.RollCharges))
            {
                rollCooldownTimer = rollCooldownLength;
            }

            else
            {
                rollIndicator.SetActive(false);
            }
        }
    }

    void Aim()
    {
        //Check if the mouse has moved
        if (previousMousePos != Input.mousePosition)
        {
            // Create a ray at the mouses position on screen
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Cast the ray from the mouse position out into the scene if hitting the ground layer turning the model to face the collision point
            if (rollTimer <= 0.0f && Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, 1 << 6))
            {
                transform.LookAt(new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z - weaponParent.position.y));
                aimPos = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z - weaponParent.position.y);
            }
            previousMousePos = Input.mousePosition;
        }
        
        // Controller aim
        else if (rollTimer <= 0.0f && (Input.GetAxisRaw("AimH") != 0 || Input.GetAxisRaw("AimV") != 0))
        {
            Vector3 direction = new Vector3(Input.GetAxisRaw("AimH"), 0, Input.GetAxisRaw("AimV"));
            transform.LookAt(transform.position + direction);
            aimPos = transform.position + direction;
        }
    }

    void ItemInteract()
    {
        //Check if player trying to interact
        if (!GameManager.GMinstance.GetInputDown("keyPickup"))
        {
            return;
        }

        // Get all objects nearby
        Collider[] cols = Physics.OverlapSphere(transform.position, 5);
        // Get the closest item to you
        int itemType = -1; // 0 = weapon, 1 = active powerup
        float closestDistance = 999.9f;
        BaseWeapon closestWeapon = null; ;
        BaseActivePowerup closestActivePowerup = null;

        // Iterate through each collider found in pick up range
        foreach (Collider col in cols)
        {
            if (closestDistance > Vector3.Distance(transform.position, col.transform.position))
            {
                // If the collider is a weapon and mark it as closest item if thats the case
                if (col.gameObject.TryGetComponent<BaseWeapon>(out BaseWeapon weapon))
                {
                    itemType = 0;
                    closestWeapon = weapon;
                    closestDistance = Vector3.Distance(transform.position, col.transform.position);
                }

                // Same as above but for abilities
                else if (col.gameObject.TryGetComponent<BaseActivePowerup>(out BaseActivePowerup activePowerup))
                {
                    if (activePowerup.IsEquipped()) continue; // Ignore abilities that are already equipped
                    itemType = 1;
                    closestActivePowerup = activePowerup;
                    closestDistance = Vector3.Distance(transform.position, col.transform.position);
                }
            }
        }

        // Pick up whatever item was found
        switch (itemType)
        {
            case 0:
                PickupWeapon(closestWeapon);
                break;
            case 1:
                PickupAbility(closestActivePowerup);
                break;
        }
    }

    // Picks up an active ability powerup
    public void PickupAbility(BaseActivePowerup a_ability)
    {
        // No current ability
        if (currentAbility == null)
        {
            // Equip ability
            currentAbility = a_ability;
            currentAbility.Equip();
        }

        // Already ability equipped
        else
        {
            // Equip new and unequip old ability
            BaseActivePowerup oldAbility = currentAbility;
            currentAbility = a_ability;

            currentAbility.Equip();
            oldAbility.Unequip();

            // If old ability was still on cooldown restart the cooldown timer for the new powerup
            // This prevents players from swapping between 2 powerups for no cooldown times
            if (abilityTimer > 0.0f)
            {
                abilityTimer = currentAbility.GetCooldownTime();
            }
        }
    }

    // Called to check if the player is using their active ability
    void ActiveAbility()
    {
        if (abilityTimer <= 0.0f) // If not on cooldown
        {
            if (!isRolling && currentAbility != null && GameManager.GMinstance.GetInputDown("keyAbility")) // If the player is pressing the button
            {
                abilityTimer = currentAbility.UseAbility(); // Use ability

                PlayAudio(abilitySounds);

                // If timer is -1 it means that the ability can no longer be used
                if (abilityTimer == -1.0f) {
                    Destroy(currentAbility.gameObject);
                    currentAbility = null;
                }
            }
        }
        else
        {
            abilityTimer -= Time.unscaledDeltaTime * (1.0f + GetModifier(ModifierType.CooldownReduction)); // Reduce ability cooldown timer
        }
    }

    // Plays the sound cue for picking up items
    public void PlayItemPickup()
    {
        PlayAudio(pickupAudio);
    }

    // Plays a given audio clips
    private void PlayAudio(List<AudioClip> audioClips)
    {
        if (audioClips.Count > 0)
        {
            //Get the audio source component and check it's not already playing something
            if (gameObject.TryGetComponent(out AudioSource audioSource) && !audioSource.isPlaying)
            {
                //Play a random audio clip from the available clips
                audioSource.clip = audioClips[Random.Range(0, audioClips.Count)];
                //Randomise the audio pitch
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                //Set Audio volume
                audioSource.volume = GameManager.GMinstance.FXVolume;
                //Play randomised audio
                audioSource.Play();
            }
        }
    }

    // Damages the player
    // All the override does is check for an achievement, actual functionality is the same as the character.
    public override void Damage(float dmg, bool ignoreImmunity = false, bool grantImmunity = true, bool ignoreResistance = false, EffectType _effectType = EffectType.None, bool _isCrit = false)
    {
        // Achievement 18 check
        if (isRolling && Time.timeScale != 1.0f)
        {
            AchievementSystem.UnlockAchievement(18);
            AchievementSystem.Init();
            SaveLoadSystem.instance.SaveAchievements();
            GameManager.GMinstance.UnlockSteamAchievement("The_Matrix");
        }

        base.Damage(dmg, ignoreImmunity, grantImmunity, ignoreResistance, _effectType, _isCrit);
    }

    // Virtual functions
    protected override void OnDamage(float dmg, bool ignoreImmunity = false, bool grantImmunity = true, bool ignoreResistance = false, EffectType type = EffectType.None, bool _isCrit = false)
    {
        // Grant player 0.1 second of immunity after being hit
        if (grantImmunity) Protect(0.1f);

        // Update achievement flags
        playerHitInLevel = true;
        playerHitInRoom = true;

        // Play hurt sound
        PlayAudio(damageAudio);

        // Visual damage flash cue
        CameraDamageEffect.instance.ActivateFlash();
    }
    protected override void Die()
    {
        //Change this to use SceneControl
        transform.position = new Vector3(0, -300, 0);

        //gameObject.GetComponent<Collider>().enabled = false;
        FreezeGameplayInput(true);
        //rb.isKinematic = false;
        GameManager.GMinstance.GameScore = score;
        GameManager.GMinstance.OnLose();

        this.BYLEBoosted = false;
        this.isPoisoned = false;
        //SceneControl.OnLose();
    }
    public override void BYLEInteraction()
    {
        //Debug.Log("Player Affected by BYLE");
        //if(PLayer has certain gun, do other stuff)

        //otherwise poison them
        if (this.isPoisoned != true)
        {
            //instance.Poison(5.0f, 2.0f, -0.4f);
            BYLEBoosted = false;
        }

        //if(this.isPoisoned && this.BYLEBoosted)
        //{
        //    BYLEBoosted = false;
        //}
    }

    // Public functions
    public void FreezeGameplayInput(bool freeze)
    {
        freezeGameplayInputs = freeze;
        //rb.isKinematic = freeze;
        rb.velocity = new Vector3(0, 0, 0);
        //CameraController.instance.SetCameraLocked(freeze);
    }

    // Check if the player currently has all their gameplay inputs frozen
    public bool GetFreezeGameplayInput()
    {
        return freezeGameplayInputs;
    }

    // Get the ratio of current health and max health
    public float GetHealthRatio()
    {
        return health / maxHealth;
    }

    // Gets the current percentage left on the roll cooldown timer
    public float GetRollCooldown()
    {
        return rollCooldownTimer / rollCooldownLength;
    }

    // Used to update the players current coin count
    public void ChangeCoinValue(int val)
    {
        coins += val;
        if (val > 0)
        {
            GameManager.GMinstance.coinsCollected += val;
            GameManager.GMinstance.IncrementSteamStatsForAchievements("Coins", val);
        }
        if (coins < 0) { coins = 0; }
        CoinCounter.instance.UpdateUI(coins);
    }

    // Checks what direction the player is moving in
    public Vector3 GetPlayerMoveDirection() { return directionOfMovement; }

    // Returns the players current coin count
    public int GetCoinValue()
    {
        return coins;
    }

    // Gets the current mouse position
    public Vector3 GetMousePos()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, 1 << 6))
        {
            return hitInfo.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    // Gets the players current movement direction relative to the players current aim
    public PlayerMoveDirection GetMovementDirection() // Returns a value equivalent to left, right, up, down or none depending on which direction the player is moving relative to their aim
    {
        //Get the movement vector of the player
        //Dot product comparison of Tranform direction
        //Pick highest dot product
        Vector3 playerVelocity = GameManager.GMinstance.GetMovementVector3();
        if(playerVelocity == Vector3.zero)
        {
            return PlayerMoveDirection.None;
        }
        playerVelocity.Normalize();
        Vector3 closestDirection = Vector3.zero;
        Vector3[] temp = new Vector3[4]; // Array containing all 4 directions
        temp[0] = transform.forward;
        temp[1] = -transform.forward;
        temp[2] = transform.right;
        temp[3] = -transform.right;
        
        // Dot product between the players current movement and all 4 directions
        float score, highscore =-100;
        foreach(Vector3 direction in temp)
        {
            score = Vector3.Dot(playerVelocity,direction);
            if(score > highscore)
            {
                highscore = score;
                closestDirection = direction;
            }
        }

        // WHichever direction had most resembled the players current move velocity then return the enumerator that is equivalent of that direction
        if(closestDirection == temp[0])
        {
            return PlayerMoveDirection.Forwards;
        }
        else if(closestDirection == temp[1])
        {
            return PlayerMoveDirection.Backwards;
        }
        else if(closestDirection == temp[2])
        {
            return PlayerMoveDirection.Right;
        }
        else
        {
            return PlayerMoveDirection.Left;
        }
    }

    // Gets the players current roll charge count
    public int GetRollCharges()
    {
        return rollCharges;
    }

    // Gets a reference to the players current active powerup
    public BaseActivePowerup GetAbility()
    {
        return currentAbility;
    }

    // Gets the current remaining time left on the active ability cooldown
    public float GetAbilityCooldownTime()
    {
        return abilityTimer;
    }

    // Puts the ability cooldown back to it's max value
    public void RestartAbilityCooldown()
    {
        abilityTimer = currentAbility.GetCooldownTime();
    }

    // Returns true if the player is rolling
    public bool IsRolling()
    {
        return isRolling;
    }

    // Sets the player character so that they have the matching abilties and modifiers of the characters on the main screen
    public void SetPlayerCharacter()
    {
        //Remove any existing character decor
        //if (decor.GetChild(0) != null) Destroy(decor.GetChild(0).gameObject);

        //Add character decor
        GameObject decorObject = null;
        GameObject characterAbility = null;
        List<Modifier> characterMods = new List<Modifier>();

        // Get current character and assign modifiers and abilities
        currCharacter = GameManager.GMinstance.selectedCharacter;
        switch (currCharacter)
        {
            case SelectedCharacter.VLAD:
                decorObject = Instantiate(Resources.Load("CharacterDecor/Vlad") as GameObject, decor);
                characterAbility = Instantiate(Resources.Load("CharacterDecor/VladMolotov") as GameObject);
                characterMods.Add(new Modifier(ModifierType.ExtraCoinChance, 0.1f));
                break;
            case SelectedCharacter.MIKE:
                decorObject = Instantiate(Resources.Load("CharacterDecor/Mike") as GameObject, decor);
                characterMods.Add(new Modifier(ModifierType.MaxHealth, 50.0f));
                characterMods.Add(new Modifier(ModifierType.ClipSize, 0.5f));
                break;
            case SelectedCharacter.OL_ONE_EYE:
                decorObject = Instantiate(Resources.Load("CharacterDecor/OneEye") as GameObject, decor);
                characterMods.Add(new Modifier(ModifierType.ReloadTime, 0.25f));
                characterMods.Add(new Modifier(ModifierType.CritChance, 0.15f));
                break;
            case SelectedCharacter.DART:
                decorObject = Instantiate(Resources.Load("CharacterDecor/Dart") as GameObject, decor);
                characterAbility = Instantiate(Resources.Load("CharacterDecor/DartPills") as GameObject);
                characterMods.Add(new Modifier(ModifierType.RollCharges, 1));
                break;
            case SelectedCharacter.KARLOS:
                decorObject = Instantiate(Resources.Load("CharacterDecor/Karlos") as GameObject, decor);
                characterMods.Add(new Modifier(ModifierType.Damage, 0.25f));
                characterMods.Add(new Modifier(ModifierType.Pierce, 1.0f));
                break;
            case SelectedCharacter.XERXES:
                decorObject = Instantiate(Resources.Load("CharacterDecor/Xerxes") as GameObject, decor);
                characterMods.Add(new Modifier(ModifierType.MaxHealth, -99.0f));
                characterMods.Add(new Modifier(ModifierType.Damage, 0.3333f));
                characterMods.Add(new Modifier(ModifierType.Speed, 0.5f));
                characterMods.Add(new Modifier(ModifierType.RollCharges, 2.0f));
                break;
            case SelectedCharacter.KYLE:
                decorObject = Instantiate(Resources.Load("CharacterDecor/Kyle") as GameObject, decor);
                characterMods.Add(new Modifier(ModifierType.ClipSize, 0.5f));
                characterMods.Add(new Modifier(ModifierType.CooldownReduction, 0.25f));
                characterMods.Add(new Modifier(ModifierType.CritChance, 0.25f));
                characterMods.Add(new Modifier(ModifierType.Damage, 0.25f));
                characterMods.Add(new Modifier(ModifierType.Discount, 0.25f));
                characterMods.Add(new Modifier(ModifierType.MaxHealth, 20.0f));
                characterMods.Add(new Modifier(ModifierType.Pierce, 1.0f));
                characterMods.Add(new Modifier(ModifierType.ProjectileSpeed, 0.25f));
                characterMods.Add(new Modifier(ModifierType.RateOfFire, 0.25f));
                characterMods.Add(new Modifier(ModifierType.ReloadTime, 0.25f));
                characterMods.Add(new Modifier(ModifierType.Speed, 0.25f));
                characterMods.Add(new Modifier(ModifierType.Vampirism, 0.02f));
                characterMods.Add(new Modifier(ModifierType.RollCharges, 2.0f));
                break;

            default: // If character somehow fails then default to Rambo's abilties
            case SelectedCharacter.RAMBO:
                decorObject = Instantiate(Resources.Load("CharacterDecor/Rambo") as GameObject, decor);
                characterMods.Add(new Modifier(ModifierType.Speed, 0.25f));
                characterMods.Add(new Modifier(ModifierType.ProjectileSpeed, 0.25f));
                break;
        }

        // Give special abilities on first level
        if (PlayerController.instance != null && GameManager.GMinstance.level == 1)
        {
            if (characterAbility == null) characterAbility = Instantiate(Resources.Load("CharacterDecor/DefaultStim") as GameObject); // If no ability was assigned give the character health stims
            PickupAbility(characterAbility.GetComponent<BaseActivePowerup>()); // Equip the ability on the player

            Debug.Log("Added Modifiers because 1st level");
            foreach (Modifier modifier in characterMods) // Apply each modifier listed onto the plauer
            {
                AddModifier(modifier.type, modifier.value);
            }
        }
    }

#region WeaponCode
    // Pickup a given weapon
void PickupWeapon(BaseWeapon _droppedWeapon)
    {
        if(_droppedWeapon == null) { return; }

        // Play swap animation for the weapon
        if(_droppedWeapon.GetSwapAnimName() != "")
        {
            animator.Play(_droppedWeapon.GetSwapAnimName());
        }
        else // Play pistol anim if missing
        {
            animator.Play("PistolSwap");
        }

        if (primaryWeapon == null && secondaryWeapon == null)
        {
            //No weapons, so add the weapon and put it in the primary slot
            //add weawpon
            primaryWeapon = _droppedWeapon;
            _droppedWeapon.OnPickup();
            //Put it in slot
            _droppedWeapon.transform.parent = weaponParent;
            _droppedWeapon.transform.position = weaponParent.position;
            _droppedWeapon.transform.rotation = weaponParent.rotation;
        }
        else if (primaryWeapon != null && secondaryWeapon == null)
        {
            //One weapon
            //add the weapon and put it in the primary slot
            //and move the current weapon to the secondary slot

            //Move weapon references
            secondaryWeapon = primaryWeapon;
            primaryWeapon = _droppedWeapon;
            primaryWeapon.OnPickup();
            //Move old weapon to secondary slot
            secondaryWeapon.transform.parent = secondaryParent;
            secondaryWeapon.transform.position = secondaryParent.position;
            secondaryWeapon.transform.rotation = secondaryParent.rotation;
            secondaryWeapon.OnStow();
            //Move new weapon to primary slot
            primaryWeapon.transform.position = weaponParent.position;
            primaryWeapon.transform.parent = weaponParent;
            _droppedWeapon.transform.rotation = weaponParent.rotation;
        }
        else if (primaryWeapon != null && secondaryWeapon != null)
        {
            //Two weapons
            //Equip new weapon in primary slot, and drop curr primary weapon

            //Remove primary weapon
            primaryWeapon.OnDrop();
            //Set new weapon
            primaryWeapon = _droppedWeapon;
            primaryWeapon.OnPickup();
            //Set pos
            primaryWeapon.transform.position = weaponParent.position;
            primaryWeapon.transform.parent = weaponParent;
            _droppedWeapon.transform.rotation = weaponParent.rotation;
        }
        else
        {
            Debug.Log("wtf");
        }

    }

    // Checks for any weapon inputs
    void UpdateWeapon()
    {
        //Check for inputs
        if(primaryWeapon == null) { return; }
        //Mouse0
        if (!isRolling && weaponSwapTime + 0.4f <= Time.unscaledTime)
        {
            if (GameManager.GMinstance.GetInput("keyShoot1"))
            {
                primaryWeapon.OnFireOneHeld();
            }
            if (GameManager.GMinstance.GetInputDown("keyShoot1"))
            {
                primaryWeapon.OnFireOneDown();
            }
            if (GameManager.GMinstance.GetInputUp("keyShoot1"))
            {
                primaryWeapon.OnFireOneUp();
            }
            //Mouse1
            if (GameManager.GMinstance.GetInput("keyShoot2"))
            {
                primaryWeapon.OnFireTwoHeld();
            }
            if (GameManager.GMinstance.GetInputDown("keyShoot2"))
            {
                primaryWeapon.OnFireTwoDown();
            }
            if (GameManager.GMinstance.GetInputUp("keyShoot2"))
            {
                primaryWeapon.OnFireTwoUp();
            }
            //Reload
            if (GameManager.GMinstance.GetInput("keyReload"))
            {
                primaryWeapon.OnReloadHeld();
            }
            if (GameManager.GMinstance.GetInputDown("keyReload"))
            {
                primaryWeapon.OnReloadDown();
            }
            if (GameManager.GMinstance.GetInputUp("keyReload"))
            {
                primaryWeapon.OnReloadUp();
            }
        }
    }

    // Switches between the 2 equipped weapons
    void SwitchWeapon()
    {
        if (secondaryWeapon == null) { return; }

        // Cannot rapidly scroll between weapons
        if (scrollTimer > 0)
        {
            scrollTimer -= Time.unscaledDeltaTime;
            return;
        }
        if (Input.mouseScrollDelta.y == 0)
        {
            return;
        }
        scrollTimer = 0.1f;

        //Swap references to the weapons
        Debug.Log("Weapon swapped");
        BaseWeapon temp = primaryWeapon;
        primaryWeapon = secondaryWeapon;
        secondaryWeapon = temp;

        //Swap positions
        primaryWeapon.transform.parent = weaponParent;
        primaryWeapon.transform.position = weaponParent.transform.position;
        primaryWeapon.transform.rotation = weaponParent.transform.rotation;

        secondaryWeapon.transform.parent = secondaryParent;
        secondaryWeapon.transform.position = secondaryParent.transform.position;
        secondaryWeapon.transform.rotation = secondaryParent.transform.rotation;

        // Equipd and stow the weapons
        primaryWeapon.OnEquip();
        secondaryWeapon.OnStow();

        // Play weapon swap animations
        animator.speed = 1;
        string animName = primaryWeapon.GetSwapAnimName();
        if(animName == null)
        {
            animator.Play("PistolSwap");
        }
        else
        {
            animator.Play(animName);
        }

        // Reset timer
        weaponSwapTime = Time.unscaledTime;
    }

    // Set the main weapon on the player. Used by game manager
    public void SetMainWeapon(GameObject weapon)
    {
        if(weapon.TryGetComponent<BaseWeapon>(out BaseWeapon newWeapon))
        {
            if(primaryWeapon != null)
            {
                //Delete the current primary weapon
                Destroy(primaryWeapon.gameObject);
            }

            //Set the new weapon to the primary weapon
            primaryWeapon = newWeapon;
            primaryWeapon.OnPickup();
                
            //Put new weapon into slot
            primaryWeapon.transform.parent = weaponParent;
            primaryWeapon.transform.position = weaponParent.position;
            primaryWeapon.transform.rotation = weaponParent.rotation;
        }
    }
    // Same as above but for secondary weapon
    public void SetSecondaryWeapon(GameObject weapon)
    {
        if(weapon.TryGetComponent<BaseWeapon>(out BaseWeapon newWeapon))
        {
            if(secondaryWeapon != null)
            {
                //Delete the current primary weapon
                Destroy(secondaryWeapon.gameObject);
            }
            
            //Set the new weapon to the primary weapon
            secondaryWeapon = newWeapon;
            secondaryWeapon.OnPickup();
                
            //Put new weapon into slot
            secondaryWeapon.transform.parent = secondaryParent;
            secondaryWeapon.transform.position = secondaryParent.position;
            secondaryWeapon.transform.rotation = secondaryParent.rotation;
        }
    }

    // Get the sprite of the main weapon for the HUD
    public Sprite GetMainWeaponSprite()
    {
        return primaryWeapon.weaponSprite;
    }

    // Returns the main / secondary weapon reference
    public BaseWeapon GetMainWeapon()
    {
        return primaryWeapon;
    }
    public BaseWeapon GetSecondaryWeapon()
    {
        return secondaryWeapon;
    }

    // Enables or disabled the ability to fire weapons
    public void EnableWeapons(bool a_enabled)
    {
        canUseWeapons = a_enabled;
    }
    #endregion

    // Resets the players animation state
    public void ResetPlayerAnimationState()
    {
        animator = this.GetComponent<Animator>(); // Get animator
        
        // Reset primarys collider and state and start idle anim
        if(primaryWeapon != null)
        {
            primaryWeapon.GetComponent<Collider>().enabled = false;
            primaryWeapon.GetComponent<BaseWeapon>().currState = BaseWeapon.WeaponState.EQUIPPED;
            animator.Play(primaryWeapon.GetIdleAnimName());
        }
        // Disable secondarys collider and stow it
        if(secondaryWeapon != null)
        {
            secondaryWeapon.GetComponent<Collider>().enabled = false;
            secondaryWeapon.OnStow();
        }
    }

    void OnDrawGizmos()
    {
        //Draw aim pos
        if(aimPos == null){return;}
        Gizmos.DrawWireCube(aimPos,Vector3.one * 0.5f);
        Gizmos.DrawLine(transform.position, aimPos);
    }

}

public enum PlayerMoveDirection
{
    Forwards,
    Backwards,
    Left,
    Right,
    None
}
