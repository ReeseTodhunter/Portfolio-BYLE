using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class RPGProjectile : MonoBehaviour, IProjectile
{
    public bool wasInit { get; set; }
    public float speed { get; set; }
    public float acceleration { get; set; }
    public float lifetime { get; set; }
    public float damage { get; set; }
    public string parentTag { get; set; }

    public float timer; // Despawn timer
    public GameObject explosionEffect;
    public float radius;

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
        //Increments the despawn timer
        timer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check that the other collider isn't the one shooting it, a trigger box or another projectile
        if (other.tag != parentTag && !other.TryGetComponent(out IProjectile iProj) && !other.isTrigger) // Target is not friendly
        {
            if (other.TryGetComponent(out BTAgent _agent))
            {
                if (!_agent.isAlive()) { return; } // goes through dead agents
            }
            Explode();
        }
    }
    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        //Get all collisions in range of the explosion
        Collider[] objects = Physics.OverlapSphere(transform.position, radius);

        //Check if each object is a character and if so damage them
        foreach (Collider obj in objects)
        {
            if (obj.gameObject.TryGetComponent<Character>(out Character character))
            {
                character.Damage(10.0F);
            }
        }
        CameraController.instance.ShakeCameraOverTime(0.5f, 1);
        Destroy(this.gameObject);
    }
}