using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    /* Required variables/functions:
     * 
     * DONE - Speed
     * DONE - Accel/Deccel
     * Relative Spawn Pos? - might not be needed as pos is given when instantiating the projectile anyway
     * Direction? - might not be needed as rotation is also given when instantiating
     * DONE - Max Lifetime
     * DONE - Damage
     * DONE - Who to damage
     * Sound?
     * Falloff? - Would likely be more complex than 1 float
     */

    bool wasInit { get; set; } // Bool to check if projectile was initialised or not, projectile should throw errors if this wasn't done >:(
    float speed { get; set; } // Initial speed of projectile on fire
    float acceleration { get; set; } // Acceleration / deccelarion of projectile after firing
    float lifetime { get; set; } // How long the projectile will exist after firing (stops projectiles existing for infinite time if they miss a target)
    float damage { get; set; } // How much damage the projectile will do upon impact with a target
    string parentTag { get; set; } // Name of tag of the character that fired projectile. Prevents friendly fire

    void Init(float a_speed, float a_acceleration, float a_lifetime, float a_damage, string a_parentTag); // Used to initialise projectile after instantiating
}

/*
public class Example : MonoBehaviour, IProjectile
{
    public float moveSpeed { get; set; }

    public void ProjectileFunction()
    {
        
    }

    void Start()
    {
        moveSpeed = 10.0f;
    }
}
*/