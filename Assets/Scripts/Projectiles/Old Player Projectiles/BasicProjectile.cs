using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class BasicProjectile : MonoBehaviour, IProjectile
{
    public bool wasInit { get; set; }
    public float speed { get; set; }
    public float acceleration { get; set; }
    public float lifetime { get; set; }
    public float damage { get; set; }
    public string parentTag { get; set; }
    float timer; // Despawn timer
    public GameObject impactVFX;
    public void Init(float a_speed, float a_acceleration, float a_lifetime, float a_damage, string a_parentTag)
    {
        wasInit = true;
        speed = a_speed;
        acceleration = a_acceleration;
        lifetime = a_lifetime;
        damage = a_damage;
        parentTag = a_parentTag;
        if (this.TryGetComponent(out VisualEffect effect))
        {
            effect.Play();
        }
        
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
    {
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
            if(impactVFX != null)
            {
                GameObject impact = Instantiate(impactVFX,transform.position,transform.rotation);
                impact.GetComponent<VisualEffect>().Play();
            }
            Destroy(gameObject);

        }
    }
}
