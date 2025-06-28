using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : MonoBehaviour, IProjectile
{
    public bool wasInit { get; set; }
    public float speed { get; set; }
    public float acceleration { get; set; }
    public float lifetime { get; set; }
    public float damage { get; set; }
    public string parentTag { get; set; }

    float timer; // Despawn timer

    public void Init(float a_speed, float a_acceleration, float a_lifetime, float a_damage, string a_parentTag)
    {
        wasInit = true;
        speed = a_speed;
        acceleration = a_acceleration;
        lifetime = a_lifetime;
        damage = a_damage;
        parentTag = a_parentTag;
    }

    void Start()
    {
        // Check if projectile was initialised
        if (!wasInit)
        {
            Debug.LogError("YOU ARE SPAWNING IN UNINITIALISED PROJECTILES FROM SOMEWHERE! THAT IS CRINGE!");
        }
        // GameObject muzzleFlash = Instantiate(ProjectileLibrary.instance.GetEffect(ProjectileEffects.MUZZLE_FLASH),transform.position,transform.rotation);
        // muzzleFlash.GetComponent<WeaponEffect>().Initialise(transform,1);
        // muzzleFlash.GetComponent<WeaponEffect>().Start();
        //this.tag = parentTag; // Setting tag of projectile (prevents collisions with other projectiles fired by the same team)
        timer = 0.0f; // Starting despawn timer for projectile
    }

    void Update()
    {
        // Movement
        transform.Translate(Vector3.forward * speed * Time.deltaTime); // Move projectile
        speed += acceleration * Time.deltaTime; // Accel/Deccelerate projectile

        // Despawn projectile
        if (timer >= lifetime)
        {
            Destroy(gameObject); // Destroy self when despawn timer reaches max lifetime
        }
        timer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {                                                                                  //Quick fix for shooting through byle. This would need to be applied to every projectile
        if (other.tag != parentTag && !other.TryGetComponent(out IProjectile iProj) && !other.isTrigger) // Target is not friendly
        {
            if(other.TryGetComponent(out BTAgent _agent))
            {
                if(!_agent.isAlive()){return;} // goes through dead agents
            }
            if (other.TryGetComponent(out Character chara))
            {
                chara.Damage(damage);
            }
            GameObject impactEffect = Instantiate(ProjectileLibrary.instance.GetEffect(ProjectileEffects.BULLET_IMPACT),transform.position,transform.rotation);
            impactEffect.GetComponent<WeaponEffect>().Initialise(transform,1);
            impactEffect.GetComponent<WeaponEffect>().Start();

            Destroy(gameObject);

        }
    }
}
