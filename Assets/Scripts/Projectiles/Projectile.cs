using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Audio;

public class Projectile : MonoBehaviour
{
    /* -- PROJECTILE BASE CLASS --
     * All projectile prefabs can either use this script or a child of it in order for it to function as a projectile
     * It can be initialised using code like Instantiate(-).GetComponent<Projectile>().Init(-)
     * Then any extra features the projectile should have (pierce, falloff, statuses) can be applied using another .SetWhatever(-) function after .Init(-)
     * Any child scripts have full access to every variable in this class and can even override some functions if necessary
     */

    // Don't forget to initialise projectiles when instantiating them. This will be set to true when you do so.
    bool wasInit = false;
    // Gameplay variables
    protected float speed = 0.0f; // Speed that the projectile will travel
    protected float acceleration = 0.0f; // Acceleration of the projectile
    protected float lifetime = 0.0f; // How long the projectile will exist after being initialised
    protected float damage = 0.0f; // How much damage the projectile will do on contact with a character
    // Pierce
    protected int pierce = 0; // How many times the projectile will pass through characters (0 = no pierce) (-1 = infinite pierce) (-2 = inf pierce + through walls)
    protected float pierceFalloff = 1.0f; // Damage multiplier for each pierce on target (1 = damage doesnt change each pierce) (0.9 = 10% less dmg each pierce) (0.5 = damage halves each pierce) etc.
    protected int pierceCount = 0; // Amount of objects the projectile has pierced through
    // Burn
    protected bool burnEffect = false; // True if the projectile will burn character on contact
    protected float burnDamage = 0.0f; // Tick damage on burn
    protected float burnTime = 0.0f; // How long the burn will last
    // Poison
    protected bool poisonEffect = false; // True if the projectile will poison character on contact
    protected float poisonDamage = 0.0f; // Tick damage on poison
    protected float poisonSpeed = 0.0f; // Speed multiplier applied when poisoned (you probably want this to be negative)
    protected float poisonTime = 0.0f; // How long the poison will last
    //Stun
    protected bool stunEffect = false;
    protected float stunTime = 0.0f;
    // Falloff
    protected bool isFalloff = false; // True if this projectile has falloff damage (damage decreased the further the projectile travels)
    protected float falloffDistStart = 0.0f; // How far the projectile must travel before falloff starts
    protected float falloffDistEnd = 0.0f; // How far the projectile must travel until falloff damage is at its lowest
    protected float falloffDamage = 0.0f; // Lowest amount of damage done by projectile / Damage done at falloffDistEnd
    // Other vars
    protected bool enabled = false;
    protected float timer = 0.0f; // Tracks how long projectile has existed for
    protected bool isPooled = false; // True if this projectile is being used in an object pool
    protected GameObject parentObject = null; // The GO that fired this projectile
    protected Character parentCharacter = null; // The character that fired this projectile
    protected string parentTag = ""; // The tag of the GO that fired this projectile
    protected Vector3 spawnPos = Vector3.zero; // Where the projectile was on spawn
    protected Vector2 direction = Vector2.zero; // The global direction that the projectile is current moving in. Changing this does not redirect the projectile!!!!!!!!
    public GameObject impactVFX = null; // VFX that plays when plays when the projectile collides with something
    public bool impactVFXOnPierce = false; // True if impact VFX plays when projectile the projectile pierces through something
    protected bool isCrit = false; //Decides if weapon does crit damage, essentialy just a different colour for damage numbers and a multipler
    protected float critMultiplier = 1; //Damage multiplier if crit
    public List<AudioClip> impactAudio = null; //Audio to play on impact
    protected bool forceCollide = false; // If set to true, the the collision check on OnTriggerEnter will always be true
    protected bool isPlayer = false; // True when the player has fired this object
    // Initialization Functions ---!! Unless the projectile isn't covered by object pooling, you probably don't need to call Init() yourself !!---
    public bool impactVFXsticksOnImpact = false; //parents the impact vfx to the collider of whatever this projectile hits, for things like spikes
    public Projectile Init(float a_speed, float a_accel, float a_lifetime, float a_damage, GameObject a_go, bool a_pool = false)
    {
        wasInit = true;
        enabled = true;

        // Set variables
        speed = a_speed;
        acceleration = a_accel;
        lifetime = a_lifetime;
        damage = a_damage;
        parentObject = a_go;
        parentTag = a_go.tag;
        spawnPos = transform.position;
        isPooled = a_pool;

        // Reset pierce
        pierceCount = 0;

        // Start VFX (if any are present)
        if (this.TryGetComponent(out VisualEffect effect)) effect.Play();

        // Set parent character (if the parent has the component)
        parentCharacter = a_go.GetComponent<Character>();

        // Check if parent is player
        if (parentObject == PlayerController.instance.gameObject) isPlayer = true;

        return this;
    }
    public Projectile SetPierce(int a_pierce, float a_dmg = 1.0f)
    {
        pierce = a_pierce;
        pierceFalloff = a_dmg;

        return this;
    }
    public Projectile SetBurn(bool a_canBurn, float a_dmg, float a_time)
    {
        burnEffect = a_canBurn;
        burnDamage = a_dmg;
        burnTime = a_time;

        return this;
    }
    public Projectile SetPoison(bool a_canPoison, float a_dmg, float a_speed, float a_time)
    {
        poisonEffect = a_canPoison;
        poisonDamage = a_dmg;
        poisonSpeed = a_speed;
        poisonTime = a_time;

        return this;
    }
    public Projectile SetStun(bool a_canStun, float a_time)
    {
        stunEffect = a_canStun;
        stunTime = a_time;

        return this;
    }
    public Projectile SetFalloff(bool a_isFalloff, float a_start, float a_end, float a_dmg)
    {
        isFalloff = a_isFalloff;
        falloffDistStart = a_start;
        falloffDistEnd = a_end;
        falloffDamage = a_dmg;

        return this;
    }
    public Projectile SetCrit(bool _isCrit, float _critMultipler) // flawless spelling
    {
        isCrit = _isCrit;
        if(_isCrit)
        {
            critMultiplier = _critMultipler;
        }
        else
        {
            critMultiplier = 1;
        }
        return this;
    }

    // Enable / disabling (for object pooling)
    public void Enable(Transform a_transform)
    {
        enabled = true;

        transform.SetPositionAndRotation(a_transform.position, a_transform.rotation); // Transport to new position
        GetComponent<SphereCollider>().enabled = true; // Reenable trigger
        timer = 0.0f; // Reset lifetime timer

        // Restart VFX
        ResetVFX();
        if (this.TryGetComponent(out VisualEffect effect)) effect.Play();
        
    }
    public void Disable()
    {
        enabled = false;

        GetComponent<SphereCollider>().enabled = false; // disable trigger
        transform.position = -1000.0f * Vector3.up; // Teleport below map

        // Stop VFX
        ResetVFX();
        if (this.TryGetComponent(out VisualEffect effect)) effect.Stop();
    }

    // Setters / Getters
    public void SetVelocity(float _velocity) { speed = _velocity; }
    public float GetVelocity() { return speed; }
    public GameObject GetParentObject() { return parentObject; }
    public float GetDistanceTraveled() { return (transform.position - spawnPos).magnitude; }

    // Update functions
    protected void Start()
    {
        if (!wasInit) Debug.Log("Uninitialised projectile \"" + gameObject.name + "\". This is CRINGE!!!"); // Init() check. Don't be cringe :)
    }
    protected virtual void Update() // Update
    {
        if (!enabled) return;
        // Player projectiles must use unscaled delta time due to time scale altering abilities

        // Move projectile forwards
        if (isPlayer) transform.Translate(Vector3.forward * speed * Time.unscaledDeltaTime);
        else transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Updating speed for acceleration
        if (isPlayer) speed += acceleration * Time.unscaledDeltaTime;
        else speed += acceleration * Time.deltaTime;

        ProjectileUpdate(); // Child updates

        if (timer >= lifetime) DestroyProjectile(); // Check if projectile has exceeded its lifetime

        // Update timer
        if (isPlayer) timer += Time.unscaledDeltaTime;
        else timer += Time.deltaTime;
    }
    protected virtual void ProjectileUpdate() // An update function that allows children to run their own Update() code without overwriting generic projectile Update() stuff
    {
        return;
    }

    // Other
    protected float GetCurrentDamage() // Returns how much damage the projectile should do at this instance (only necessary really for falloff)
    {
        float damageCalc = damage;

        if (isFalloff) // Calculate falloff if present
        {
            float distanceTravelled = (transform.position - spawnPos).magnitude;

            if (distanceTravelled >= falloffDistEnd) damageCalc = falloffDamage; // Projectile has travelled max distance and damage is fully reduced
            else if (distanceTravelled >= falloffDistStart)
            {
                // Lerp between two damage values based off how far projectile has moved between the 2 falloff distances
                float distAcrossFalloff = (distanceTravelled - falloffDistStart) / (falloffDistEnd - falloffDistStart);
                float damageChange = (damage - falloffDamage) * distAcrossFalloff;
                damageCalc -= damageChange;
            }
        }

        if (pierceCount > 0 && pierceFalloff != 1.0f) // Calculate pierce falloff if necessary
        {
            for (int i = 0; i < pierceCount; ++i)
            {
                damageCalc *= pierceFalloff;
            }
        }

        return damageCalc * critMultiplier; // Return calculated damage
    }
    public virtual void DestroyProjectile(GameObject projectileDestroyer = null) // Called when projectile should kill itself NOW
    {
        // If object is pooled then disable the projectile
        if (isPooled)
        {
            Disable();
        }

        // If not pooled then just destroy this game object
        else
        {
            Destroy(gameObject);
        }
    }
    public void ForceDestroyObject()
    {
        DestroyProjectile();
    }
    public string GetParentTag()
    {
        return parentTag;
    }

    protected void ResetVFX()
    {
        // Clear trail renderers
        if (transform.TryGetComponent(out TrailRenderer trail)) trail.Clear();
        foreach (Transform childTransform in transform) { if (childTransform.TryGetComponent(out TrailRenderer childTrail)) childTrail.Clear(); }

        // Clear particle systems
        if (transform.TryGetComponent(out ParticleSystem particles))
        {
            particles.Clear();
            particles.Play();
        }
        foreach (Transform childTransform in transform)
        {
            if (childTransform.TryGetComponent(out ParticleSystem childParticles))
            {
                childParticles.Clear();
                childParticles.Play();
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other) // Called when something enters the projectiles trigger zone
    {
        // Check if the collider is not: on the same team as the parent, not a projectile, not a trigger and not the floor
        if (forceCollide || (other.gameObject != parentObject && !other.gameObject.CompareTag(parentTag) && !other.TryGetComponent(out Projectile _proj) && !other.isTrigger && other.gameObject.layer != 6))
        {
            // If collider is a dead enemy, pass through and ignore
            if (other.TryGetComponent(out BTAgent _agent))
            {
                if (!_agent.isAlive()) { return; } // Goes through dead AIs
            }

            // If collider has a character component
            if (other.TryGetComponent(out Character chara))
            {
                EffectType type = EffectType.None;
                // Damage character
                if (burnEffect){chara.Burn(burnTime, burnDamage); type = EffectType.Burn;};
                if(stunEffect){chara.Stun(stunTime); type = EffectType.Stun;};
                if (!PlayerController.instance.GetComponent<Character>().IsPoisoned())
                {
                    if (poisonEffect) chara.Poison(poisonTime, poisonDamage, poisonSpeed);
                }
                
                chara.Damage(GetCurrentDamage(),false,true,false,type,isCrit);

                // Any on-damage effects
                if (parentCharacter != null) parentCharacter.OnDamageDealt(GetCurrentDamage());

                // Play impact VFX
                if (impactVFX != null && ((pierceCount == pierce) || impactVFXOnPierce))
                {
                    GameObject impact = Instantiate(impactVFX, transform.position, transform.rotation);
                    if(impactVFXsticksOnImpact)
                    {
                        impact.transform.parent = other.transform;
                        impact.layer = other.gameObject.layer;
                    }
                    if (impact.TryGetComponent<VisualEffect>(out VisualEffect vfx))
                    {
                        vfx.Play();
                    }
                    if (impact.TryGetComponent<ParticleSystem>(out ParticleSystem pSystem))
                    {
                        pSystem.Play();
                    }
                    //Play character damage Audio
                    if (impactAudio != null)
                    {
                        //if(impact.TryGetComponent<AudioSource>(out AudioSource source))
                        //{
                        //    //Please change me
                        //    source.clip = impactAudio[Random.Range(0, impactAudio.Count)];
                        //    //impact.GetComponent<AudioSource>().time = 0.3f;
                        //    source.Play();
                        //}
                    }
                }

                // Kill projectile or update pierce counters (depending on what state the projectiles pierce currently is)
                if (pierceCount == pierce) DestroyProjectile(other.gameObject);
                else if (pierceCount < pierce) pierceCount++;
            }

            // Collider hit something else (probably a wall)
            else if (pierce != -2)
            {
                // Play impact VFX
                if (impactVFX != null && ((pierce == 0) || impactVFXOnPierce))
                {
                    GameObject impact = Instantiate(impactVFX, transform.position, transform.rotation);
                    if(impactVFXsticksOnImpact)
                    {
                        impact.transform.parent = other.transform;
                        impact.layer = other.gameObject.layer;
                    }
                    if (impact.TryGetComponent<VisualEffect>(out VisualEffect vfx))
                    {
                        vfx.Play();
                    }
                    if (impact.TryGetComponent<ParticleSystem>(out ParticleSystem pSystem))
                    {
                        pSystem.Play();
                    }
                    //Play impact Audio
                    if (impactAudio.Count > 0)
                    {
                        impact.GetComponent<AudioSource>().clip = impactAudio[Random.Range(0, impactAudio.Count)];
                        impact.GetComponent<AudioSource>().volume = GameManager.GMinstance.FXVolume;
                        impact.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                        impact.GetComponent<AudioSource>().Play();
                    }
                }

                // Kill projectile unless pierce allows for projectile to travel through walls
                DestroyProjectile(); 
            }
        }
    }
}
