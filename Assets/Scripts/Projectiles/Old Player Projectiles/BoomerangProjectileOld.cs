using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class BoomerangProjectileOld : MonoBehaviour, IProjectile
{
    public bool wasInit { get; set; }
    public float speed { get; set; }
    public float acceleration { get; set; }
    public float lifetime { get; set; }
    public float damage { get; set; }
    public string parentTag { get; set; }

    public float dist = 10; //distance to travel before looping back

    float timer; // Despawn timer
    public float SmoothTurnTime = 0.1f; //smooths out the projectile's turn
    
    float turnSmoothVelocity; //Speed to turn projectile at
    Vector3 targetPos;
    bool returning;

    public void Init(float a_speed, float a_acceleration, float a_lifetime, float a_damage, string a_parentTag)
    {
        wasInit = true;
        speed = a_speed;
        acceleration = a_acceleration;
        lifetime = a_lifetime;
        damage = a_damage;
        parentTag = a_parentTag;
        if(TryGetComponent<VisualEffect>(out VisualEffect vfx))
        {
            vfx.Play();
        }
    }

    void Start()
    {
        // Check if projectile was initialised
        //this.tag = parentTag; // Setting tag of projectile (prevents collisions with other projectiles fired by the same team)
        returning = false;  //Ensure the boomerang doesn't immediately return
        timer = 0.0f; // Starting despawn timer for projectile
    }

    void Update()
    {
        targetPos = PlayerController.instance.gameObject.transform.position;
        // Movement
        transform.Translate(Vector3.forward * speed * Time.deltaTime); // Move projectile
        speed += acceleration * Time.deltaTime; // Accel/Deccelerate projectile

        if (Vector3.Distance(targetPos, transform.position) > dist && !returning)
        {
            returning = true;
        }
        if (returning)
        {
            //Turn boomerang slowly to return to it's origin of fire
            Vector2 temp = new Vector2(targetPos.x - transform.position.x, targetPos.z - transform.position.z);
            
            float targetAngle = Mathf.Atan2(temp.x, temp.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, SmoothTurnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (Vector3.Distance(transform.position, targetPos) < 0.75f)
            {
                Destroy(gameObject); // Destroy self when despawn timer reaches max lifetime
            }
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
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
