using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardProjectile : MonoBehaviour, IProjectile
{
    public bool wasInit { get; set; }
    public float speed { get; set; }
    public float acceleration { get; set; }
    public float lifetime { get; set; }
    public float damage { get; set; }
    public string parentTag { get; set; }

    float timer; // Despawn timer
    public float height = 5.0f; //How high the midpoint should be
    public GameObject Hazard; //Hazard to spawn when hitting the floor

    float turnSmoothVelocity; //Speed to turn projectile at
    Vector3 targetPos, midPos;
    bool falling;

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
        GameObject muzzleFlash = Instantiate(ProjectileLibrary.instance.GetEffect(ProjectileEffects.MUZZLE_FLASH), transform.position, transform.rotation);
        muzzleFlash.GetComponent<WeaponEffect>().Initialise(transform, 1);
        muzzleFlash.GetComponent<WeaponEffect>().Start();

        targetPos = PlayerController.instance.GetMousePos();
        midPos = ((targetPos - transform.position) / 2) + transform.position;
        midPos.y += height;
        falling = false;  //Ensure the boomerang doesn't immediately return
        timer = 0.0f; // Starting despawn timer for projectile
    }

    void Update()
    {
        // Movement
        transform.Translate(Vector3.forward * speed * Time.deltaTime); // Move projectile
        speed += acceleration * Time.deltaTime; // Accel/Deccelerate projectile

        if (Vector3.Distance(midPos, transform.position) <= 0.2f && !falling)
        {
            falling = true;
        }
        else
        {
            transform.LookAt(midPos);
        }
        if (falling)
        {
            transform.LookAt(targetPos);
        }

        // Despawn projectile
        if (timer >= lifetime)
        {
            Destroy(gameObject); // Destroy self when despawn timer reaches max lifetime
        }
        timer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != parentTag && !other.TryGetComponent(out IProjectile iProj) && !other.isTrigger) // Target is not friendly
        {
            if (other.TryGetComponent(out Character chara))
            {
                chara.Damage(damage);
            }
            else if (other.gameObject.layer == 6)
            {
                if (Hazard != null)
                {
                    Instantiate(Hazard, transform.position, transform.rotation);
                }
            }
            GameObject impactEffect = Instantiate(ProjectileLibrary.instance.GetEffect(ProjectileEffects.BULLET_IMPACT), transform.position, transform.rotation);
            impactEffect.GetComponent<WeaponEffect>().Initialise(transform, 1);
            impactEffect.GetComponent<WeaponEffect>().Start();
            Destroy(gameObject);

        }
    }
}
