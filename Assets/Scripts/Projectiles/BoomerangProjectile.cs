using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangProjectile : Projectile
{
    private float returnDistance = 0.0f; // How far the projectile will travel before arcing back
    private float turnTime = 0.1f; // Turn time. Magic numbers are cringe but it's best this stays the same
    private float turnVelocity; // I don't know what this does I'll be honest I didn't make the original boomerang code
    private bool returning = false; // When true the projectile

    public Projectile InitBoomerang(float a_distance)
    {
        returnDistance = a_distance;

        return this;
    }

    protected override void ProjectileUpdate()
    {
        // Check if projectile has moved far enough to start returning
        if (Vector3.Distance(spawnPos, transform.position) > returnDistance) returning = true;

        if (returning && parentObject != null)
        {
            // Get position of projectile firer
            Vector3 targetPos = parentObject.transform.position;

            // Turn boomerang slowly to return to the projectile firer
            Vector2 temp = new Vector2(targetPos.x - transform.position.x, targetPos.z - transform.position.z);

            float targetAngle = Mathf.Atan2(temp.x, temp.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // If projectile has reached target destroy self
            if (Vector3.Distance(transform.position, targetPos) < 0.75f) DestroyProjectile();
        }
    }
}
