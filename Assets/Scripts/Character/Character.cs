using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public int characterID;
    // Button above player that can be used to display a debug message
    // Off by default in hierarchy
    [SerializeField] public Text debugButton;

    // Character Stats
    [SerializeField] protected float baseMaxHealth; // Max health before modifiers
    protected float maxHealth; // Max health after modifiers
    protected float health = 0.0f; // Current health of character

    // Speed
    [SerializeField] float speedDefault = 0.0f; // Speed value without any modifiers attached
    protected float speed = 0.0f; // Speed of character after modifiers

    // Immunity
    [SerializeField] GameObject immuneIndicator;
    protected bool isInvulnerable = false; // True if the character is currently immune to all damage
    float invulnerableTimer = 0.0f; // How long the character is invulnerable for

    // Status Effects
    // General
    List<StatusEffect> statusEffects = new List<StatusEffect>(); // List of every status effect on the character
    List<GameObject> statusIndicators = new List<GameObject>(); // Currently active indicators indicating the characters statuses. Indication.
    // Burn
    [SerializeField] GameObject burnIndicator;
    bool isBurning = false;
    // Poison
    [SerializeField] GameObject poisonIndicator;
    protected bool isPoisoned = false;
    protected bool isStunned = false;
    // Byle
    [SerializeField]
    public bool BYLEBoosted = false;
    //Score
    public int score = 0;

    // Attributes / Modifiers
    List<Modifier> modifiers = new List<Modifier>(); // Modifiers

    //Particle Effects
    private GameObject burnVFX;
    private GameObject stunVFX;
    private GameObject poisonVFX;


    // HEALTH
    public virtual void Damage(float dmg, bool ignoreImmunity = false, bool grantImmunity = true, bool ignoreResistance = false, EffectType _effectType = EffectType.None, bool _isCrit = false)
    {
        if(health > 0.0f)
        {
            if (!isInvulnerable || (isInvulnerable && ignoreImmunity))
            {
                float damageTaken = 0.0f;
                if (ignoreResistance)
                {
                    damageTaken = Mathf.Max(0.0f, dmg);
                }
                else
                {
                    damageTaken = Mathf.Max(0.0f, dmg * Mathf.Max(0.1f, (1.0f + GetModifier(ModifierType.Vulnerability))));
                }

                health -= damageTaken;
                if (health <= 0.0f)
                {
                    Die();
                }

                // TEMPORARY TEXT FOR DEBUGGING :))))
                if (dmg != 0)
                {
                    GameObject floatingText = Instantiate(ProjectileLibrary.instance.GetProjectile(Projectiles.FLOATING_TEXT), transform.position + Vector3.up * 1.5f + Vector3.one * Random.Range(-0.2f, 0.2f), Quaternion.Euler(Vector3.zero));
                    floatingText.GetComponent<ItemPickupFade>().SetText((Mathf.Round(damageTaken*10.0f)/10.0f).ToString(), _effectType, _isCrit);
                }

                OnDamage(damageTaken, ignoreImmunity, grantImmunity, ignoreResistance, _effectType,_isCrit);
            }
        }
        

        // Check if dead
        //Maybe Remove this later?
        if (health <= 0.0f && !isInvulnerable)
        {
            Die();
        }
    }
    public float Heal(float heal)
    {
        float oldHealth = health;

        health += Mathf.Max(0.0f, heal);
        health = Mathf.Min(health, maxHealth); // Making sure health didn't go over cap
        OnHeal(heal);
        
        if (CameraDamageEffect.instance != null)
        {
            if (GetHealth()>GetMaxHealth()/2) CameraDamageEffect.instance.SetMinOpacity();
        }
        return health - oldHealth;
    }
    public virtual void OnHeal(float _heal){}
    public float GetHealth()
    {
        return health;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetSpeed()
    {
        return speed;
    }
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
    protected virtual void OnDamage(float dmg, bool ignoreImmunity = false, bool grantImmunity = true, bool ignoreResistance = false, EffectType type = EffectType.None, bool _isCrit = false)
    {
        return;
    }

    // DAMAGE
    public virtual void OnDamageDealt(float dmg)
    {
        Heal(dmg * GetModifier(ModifierType.Vampirism));
    }

    // BYLE
    public virtual void BYLEInteraction()
    {
        //Override this in the player character and enemy scripts
        //For enemies, this may not work due to the behaviour tree
    }

    // INVULNERABILITY
    public void Protect(float time)
    {
        if (invulnerableTimer < time) { invulnerableTimer = time; }
        isInvulnerable = true;
        UpdateIndicator(immuneIndicator, true);
    }
    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }

    // STATUS EFFECTS
    private void AddStatus(StatusType a_type, EffectType a_effect, float a_damage, float a_maxTime, float a_tickTime = 1.0f)
    {
        // Should not be called directly
        // Also do not add 2 status effects of the same "effect" type (except "None") and different lengths
        // This WILL mess up how the character knows what status effect it has
        // And it will be YOUR fault and definitely not mine
        // That being said if you want to improve it go ahead
        statusEffects.Add(new StatusEffect(a_type, a_effect, this, a_damage, a_maxTime, a_tickTime));
    }
    public void Burn(float time, float tickDmg)
    {
        // Check if the character is currently invulnerable to everything
        if (!isInvulnerable)
        {
            // Check if the character is already burning and determine if pre-existing burn should be overwritten
            if (isBurning)
            {
                // Find the other matching status effect
                List<StatusEffect> removedStatuses = new List<StatusEffect>();
                foreach(StatusEffect effect in statusEffects)
                {
                    if (effect != null && effect.MatchingStatus(StatusType.Damage, EffectType.Burn)) // Check for a damage & burn status
                    {
                        // Return early if time remaining on current burn is longer than new burn, so new burn is ignored
                        if (effect.TimeRemaining() >= time)
                        {
                            return;
                        }

                        removedStatuses.Add(effect); // Remove old status since it will now be replaced
                    }
                }
                // Clearing overwritten status effects from list
                foreach (StatusEffect effect in removedStatuses)
                {
                    statusEffects.Remove(effect);
                }
            }
            
            // Apply status effect
            AddStatus(StatusType.Damage, EffectType.Burn, tickDmg, time, 0.5f); // MAGIC NUMBER (tick time)

            // Other values
            isBurning = true;
            UpdateIndicator(burnIndicator, true);
        }
    }
    public bool IsBurning()
    {
        return isBurning;
    }
    public void Poison(float time, float tickDmg, float speedMultiplier)
    {
        if (!isInvulnerable)
        {
            // Check if the character is already burning and determine if pre-existing poison should be overwritten
            if (isPoisoned)
            {
                // Find the other matching status effects
                bool overwriteEffect = false;
                List<StatusEffect> removedStatuses = new List<StatusEffect>();
                foreach (StatusEffect effect in statusEffects)
                {
                    if (effect != null && effect.GetEffectType() == EffectType.Poison)
                    {
                        // Return early if time remaining on current poison is longer than new poison, so new poison is ignored
                        // In theory there shouldn't be 2 different poison effects with different times
                        // If there is then this overwrite code is flawed and could potentially allow for stacked poison effects
                        // But that shouldn't happen as long as AddEffect() is used properly :)
                        if (!overwriteEffect && effect.TimeRemaining() >= time)
                        {
                            return;
                        }
                        else
                        {
                            overwriteEffect = true;
                            removedStatuses.Add(effect);
                        }
                    }
                }
                // Clearing overwritten status effects from list
                foreach (StatusEffect effect in removedStatuses)
                {
                    statusEffects.Remove(effect);
                }
            }

            // Apply status effect
            AddStatus(StatusType.Damage, EffectType.Poison, tickDmg, time, 1.0f); // MAGIC NUMBER (tick time)
            AddStatus(StatusType.Speed, EffectType.Poison, speedMultiplier, time);

            // Other values
            isPoisoned = true;
            UpdateIndicator(poisonIndicator, true);
        }
    }
    public bool IsPoisoned()
    {
        return isPoisoned;
    }
    public virtual void Stun(float time)
    {
        if (!isInvulnerable)
        {
            // Check if the character is already burning and determine if pre-existing poison should be overwritten
            if (isStunned)
            {
                // Find the other matching status effects
                bool overwriteEffect = false;
                List<StatusEffect> removedStatuses = new List<StatusEffect>();
                foreach (StatusEffect effect in statusEffects)
                {
                    if (effect != null && effect.GetEffectType() == EffectType.Stun)
                    {
                        // Return early if time remaining on current poison is longer than new poison, so new poison is ignored
                        // In theory there shouldn't be 2 different poison effects with different times
                        // If there is then this overwrite code is flawed and could potentially allow for stacked poison effects
                        // But that shouldn't happen as long as AddEffect() is used properly :)
                        if (!overwriteEffect && effect.TimeRemaining() >= time)
                        {
                            return;
                        }
                        else
                        {
                            overwriteEffect = true;
                            removedStatuses.Add(effect);
                        }
                    }
                }
                // Clearing overwritten status effects from list
                foreach (StatusEffect effect in removedStatuses)
                {
                    statusEffects.Remove(effect);
                }
            }

            // Apply status effect
            AddStatus(StatusType.inputLocked, EffectType.Stun, 0, time, 1.0f); // MAGIC NUMBER (tick time)
            // Other values
            isStunned = true;
        }
    }
    public bool IsStunned()
    {
        return isStunned;
    }
    protected virtual void OnStunEnd()
    {
    }
    public void KnockbackFromPos(Vector3 sourceWorldPos, float force)
    {
        Vector3 direction = (transform.position - sourceWorldPos).normalized;
        GetComponent<Rigidbody>().velocity = direction * force;
    }
    public void KnockbackFromDir(Vector3 direction, float force)
    {
        GetComponent<Rigidbody>().velocity = direction.normalized * force;
    }

    // ATTRIBUTE MODIFIERS
    public void AddModifier(ModifierType modType, float modAmount)
    {
        modifiers.Add(new Modifier(modType, modAmount));

        // Any other active effects caused by adding modifiers
        switch (modType)
        {
            case ModifierType.MaxHealth:
                maxHealth = Mathf.Max(1.0f, baseMaxHealth + GetModifier(ModifierType.MaxHealth));
                Heal(modAmount);
                if (health > maxHealth) health = maxHealth; // Lower health if it is greater than the max
                break;
        }
    }
    public float GetModifier(ModifierType modType)
    {
        float modTotal = 0.0f;

        // really cringe rate of fire specific multiplicative modifer thing
        if (modType == ModifierType.RateOfFire)
        {
            modTotal = 1.0f;
            foreach (Modifier modifier in modifiers)
            {
                if (modifier.type == modType) modTotal *= 1-modifier.value;
            }
            return modTotal;
        }

        foreach (Modifier modifier in modifiers)
        {
            if (modifier.type == modType) modTotal += modifier.value;
        }
        
        return modTotal;
    }
    public void RemoveModifier(ModifierType modType, float modAmount)
    {
        // Attempt to find modifer keyvaluepair inside dictionary and remove it
        foreach (Modifier modifier in modifiers)
        {
            if (modifier.type == modType && modifier.value == modAmount)
            {
                modifiers.Remove(modifier);
                return;
            }
        }

        // Add negative modifier if item couldn't be removed
        Debug.LogWarning("MODIFIER OF TYPE: " + modType.ToString() + " AND VALUE: " + modAmount.ToString() + " COULD NOT BE REMOVED. ADDING NEGATIVE MODIFIER.");
        modifiers.Add(new Modifier(modType, -modAmount));
    }
    public List<Modifier> GetModifierList()
    {
        return modifiers;
    }
    // UTILITY
    protected void UpdateIndicator(GameObject indicator, bool enabled) // Used for updating status effect icons
    {
        if (indicator != null)
        {
            if (enabled && !statusIndicators.Contains(indicator))
            {
                statusIndicators.Add(indicator);
                indicator.SetActive(true);
            }
            else if (!enabled && statusIndicators.Contains(indicator))
            {
                statusIndicators.Remove(indicator);
                indicator.SetActive(false);
            }
        }
    }

    // CHARACTER UPDATES
    private void Awake()
    {
        CharacterAwake();
    }
    protected void CharacterAwake()
    {
        if(this.gameObject.GetComponent<BTAgent>() != null)
        {
            if (this.GetComponent<BTAgent>().enemyType == BTAgent.EnemyType.Boss)
            {
                baseMaxHealth = baseMaxHealth + (Mathf.CeilToInt((baseMaxHealth / 6) * GameManager.GMinstance.healthDifficultyScale));
            }
            else
            {
                Debug.Log("Enemies Health scaled with level");
                baseMaxHealth = baseMaxHealth + (Mathf.CeilToInt((baseMaxHealth / 10) * GameManager.GMinstance.healthDifficultyScale));
                Debug.Log(health);
            }
            
        }
        health = baseMaxHealth;
        speed = speedDefault;


        //init debuff effects
        burnVFX = GameObject.Instantiate(Resources.Load("VFX/Debuffs/Burn") as GameObject,Vector3.zero,Quaternion.identity);
        burnVFX.transform.parent = this.transform;
        burnVFX.transform.localPosition = Vector3.zero;
        burnVFX.gameObject.layer = transform.gameObject.layer;

        stunVFX = GameObject.Instantiate(Resources.Load("VFX/Debuffs/Stun") as GameObject,Vector3.zero,Quaternion.identity);
        stunVFX.transform.parent = this.transform;
        stunVFX.transform.localPosition = Vector3.zero;
        stunVFX.gameObject.layer = transform.gameObject.layer;

        poisonVFX = GameObject.Instantiate(Resources.Load("VFX/Debuffs/Poison") as GameObject,Vector3.zero,Quaternion.identity);
        poisonVFX.transform.parent = this.transform;
        poisonVFX.transform.localPosition = Vector3.zero;
        poisonVFX.gameObject.layer = transform.gameObject.layer;
    
    }

    private void Update()
    {
        CharacterUpdate(); 
    }
    protected void CharacterUpdate()
    {
        // HEALTH
        maxHealth = Mathf.Max(1.0f, baseMaxHealth + GetModifier(ModifierType.MaxHealth));
        if (health > maxHealth) health = maxHealth; // Lower health if it is greater than the max

        // INVULNERABILITY
        if (isInvulnerable)
        {
            invulnerableTimer -= Time.deltaTime;
            if (invulnerableTimer <= 0.0f)
            {
                isInvulnerable = false;
                UpdateIndicator(immuneIndicator, false);
            }
        }

        //status vfx update
        UpdateDebuffParticleSystem(burnVFX,isBurning);
        UpdateDebuffParticleSystem(poisonVFX,isPoisoned);
        UpdateDebuffParticleSystem(stunVFX,isStunned, true);

        //BYLE
        if (BYLEBoosted)
        {
            //Generic Override, implement individual interactions in the character scripts
            BYLEInteraction();
            //Make sure every BYLEInteraction override has a flag to turn off BYLE Boost
            //BYLEBoosted = false;
        }

        // STATUS EFFECTS
        // Resetting speed modifier, as it will be changed by the following status effects (if any)
        List<StatusEffect> removedStatuses = new List<StatusEffect>();
        // Updating each status effect
        foreach (StatusEffect effect in statusEffects)
        {
            if (effect == null) {
                removedStatuses.Add(effect);
                continue;
            }

            effect.Update();

            if (effect.IsOver())
            {
                switch (effect.GetEffectType()) {
                    case EffectType.Burn:
                        isBurning = false;
                        UpdateIndicator(burnIndicator, false);
                        break;
                    case EffectType.Poison:
                        isPoisoned = false;
                        UpdateIndicator(poisonIndicator, false);
                        break;
                    case EffectType.Stun:
                        isStunned = false;
                        break;
                }
                removedStatuses.Add(effect);
                continue;
            }
        }
        // Clearing all finished status effects from list
        foreach (StatusEffect effect in removedStatuses)
        {
            statusEffects.Remove(effect);
            if(effect.GetEffectType() == EffectType.Stun){OnStunEnd();}
        }
        // Updating UI for status effects
        for (int i = 0; i < statusIndicators.Count; ++i)
        {
            GameObject indicator = statusIndicators[i];
            float width = indicator.GetComponent<RectTransform>().sizeDelta.x; // Width of the indicator icon. If the size of these aren't all equal this will probably break
            float spacing = 55.0f;

            // Calculates the position of that indicator so all that are active on the character are centred nicely
            indicator.transform.localPosition = new Vector3(spacing*i - 0.5f*(statusIndicators.Count-1)*spacing, indicator.transform.localPosition.y, indicator.transform.localPosition.z);
        }

        // Updating for the characters new speed with modifiers applied
        speed = speedDefault * Mathf.Max(0.0f, 1.0f + GetModifier(ModifierType.Speed));
    }
    private void UpdateDebuffParticleSystem(GameObject pSystem, bool _debuffState, bool _clearParticles = false)
    {
        if(!_debuffState)
        {
            pSystem.GetComponent<ParticleSystem>().Stop();
            if(_clearParticles){pSystem.GetComponent<ParticleSystem>().Clear();}
            return;
        }
        if(pSystem.GetComponent<ParticleSystem>().isPlaying){return;}
        pSystem.GetComponent<ParticleSystem>().Play();
    }
}


public class Modifier
{
    public Modifier(ModifierType type, float value)
    {
        this.type = type;
        this.value = value;
    }

    public ModifierType type { get; set; }
    public float value { get; set; }
}

// Different types of modifiers that the character can have changed against it
public enum ModifierType
{
    Accuracy,
    Damage,
    MaxHealth,
    RateOfFire,
    Speed,
    Vulnerability,
    ProjectileSpeed,
    ProjectileSize,
    ReloadTime,
    ReloadForgiveness,
    ClipSize,
    Vampirism,
    DodgeRollRing,
    RollCharges,
    Pierce,
    CoinHeal,
    EnemyExplosion,
    CritChance,
    Discount,
    ExtraCoinChance,
    CooldownReduction
}