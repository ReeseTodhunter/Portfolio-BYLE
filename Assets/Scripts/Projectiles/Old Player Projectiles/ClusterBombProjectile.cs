using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class ClusterBombProjectile : MonoBehaviour, IProjectile
{
    public bool wasInit { get; set; }
    public float speed { get; set; }
    public float acceleration { get; set; }
    public float lifetime { get; set; }
    public float damage { get; set; }
    public string parentTag { get; set; }

    public int numShrapnel = 16;
    public GameObject childProjectile;
    public GameObject explosionPrefab;
    float timer; // Despawn timer

    public void Init(float a_speed, float a_acceleration, float a_lifetime, float a_damage, string a_parentTag)
    {
        wasInit = true;
        speed = a_speed;
        acceleration = a_acceleration;
        lifetime = a_lifetime;
        damage = a_damage;
        parentTag = a_parentTag;
        this.GetComponent<VisualEffect>().Play();
    }

    void Start()
    {
        // Check if projectile was initialised
        if (!wasInit)
        {
            Debug.LogError("YOU ARE SPAWNING IN UNINITIALISED PROJECTILES FROM SOMEWHERE! THAT IS CRINGE!");
        }
        //this.tag = parentTag; // Setting tag of projectile (prevents collisions with other projectiles fired by the same team)
        timer = 0.0f; // Starting despawn timer for projectile
    }

    // Update is called once per frame
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
            if (other.TryGetComponent(out Character chara) )
            {
                chara.Damage(damage);
            }

            float spreadAngle = 360 / numShrapnel;
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            for (int i = 0; i < numShrapnel; i++)
            {
                GameObject child = Instantiate(childProjectile, transform.position, Quaternion.Euler(0, transform.rotation.eulerAngles.y + (spreadAngle * i), 0));
                child.GetComponent<IProjectile>().Init(20, -10, 2, damage / 5, parentTag);
            }

            Destroy(gameObject);
        }
    }
}