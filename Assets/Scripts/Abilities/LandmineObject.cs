using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmineObject : MonoBehaviour
{
    [SerializeField] GameObject explosionEffect;

    private LandmineAbility parent;
    private float damage;
    private float radius;
    private float armTime;

    private float timer = 0.0f;
    private bool armed = false;

    public void SetLandmine(LandmineAbility a_parent, float a_damage, float a_radius, float a_armTime)
    {
        parent = a_parent;
        damage = a_damage;
        radius = a_radius;
        armTime = a_armTime;

        timer = 0.0f;
        armed = false;
    }

    private void Update()
    {
        // Landmine will not explode for the first armTime amount of seconds after it was put down
        if (timer < armTime)
        {
            timer += Time.unscaledDeltaTime;
        }
        else
        {
            armed = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // If collider was an enemy and the landmine has been armed then it will explode
        if (armed && other.TryGetComponent(out BTAgent enemy))
        {
            // Get all collisions in range of the explosion
            Collider[] objects = Physics.OverlapSphere(transform.position, radius);

            // Check if each object is a character and if so damage them
            foreach (Collider obj in objects)
            {
                if (obj.gameObject.TryGetComponent(out Character character))
                {
                    if (character.gameObject == PlayerController.instance.gameObject) continue; // Don't hurt player
                    character.Damage(damage);
                }
            }

            // If there is an explosion effect instantiate the effect
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, transform.rotation);
            }

            // Shake camera
            CameraController.instance.ShakeCameraOverTime(0.5f, 1);

            // Start cooldown
            PlayerController.instance.RestartAbilityCooldown();

            // Destroy the landmine, goodbye cruel world
            Destroy(gameObject);
        }
    }
}
