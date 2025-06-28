using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrailModifier : BTModifier
{
    public GameObject FireProjectile;
    private float pathTimer;
    public float pathInterval = 0.25f;
    public float pathDuration = 5.0f;

    public override void Initialise(Character _agent)
    {
        title = "Firewalker";
        FireProjectile = Resources.Load("Projectiles/FlameProjectile 1") as GameObject;
        type = modifierType.onUpdate;
        base.Initialise(_agent);
    }
    public override void ActivateModifier(Character _agent)
    {
        base.ActivateModifier(_agent);
        pathTimer += Time.deltaTime;
        if (pathTimer > pathInterval)
        {
            //Instantiate Fire projectiles once every 0.5 seconds
            GameObject currPath = Instantiate(FireProjectile, _agent.transform.position, Quaternion.identity);
            currPath.AddComponent<KillScript>();
            currPath.GetComponent<Projectile>().Init(0.0f, 0.0f, pathDuration, 1.0f, _agent.gameObject, false);
            currPath.GetComponent<Projectile>().SetBurn(true, 1.0f, pathDuration);
            pathTimer = 0.0f;

        }

    }

}
