using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardProjectile : Projectile
{
    protected GameObject ignoredObject = null;
    public Projectile InitIgnoredObject(GameObject a_ignored)
    {
        ignoredObject = a_ignored;
        return this;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ignoredObject) return; // Ignore character that the main projectile shattered on
        base.OnTriggerEnter(other);
    }
}
