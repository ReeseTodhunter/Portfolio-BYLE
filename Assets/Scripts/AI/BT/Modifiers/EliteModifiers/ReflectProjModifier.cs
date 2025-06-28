using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectProjModifier : BTModifier
{
    private GameObject reflectProjectile;

    public int reflectAmount = 5;
    public float reflectSpread = 90.0f;

    private float damageAbsorbed;
    public float damageLimit = 5;
    public override void Initialise(Character _agent)
    {
        title = "Damage Reflection";
        type = modifierType.onDamage;
        reflectProjectile = Resources.Load("Projectiles/ReflectProjectile") as GameObject;
        base.Initialise(_agent);
    }

    public override void ActivateModifier(Character _agent)
    {
        base.ActivateModifier(_agent);
        damageAbsorbed += _agent.GetComponent<BTAgent>().lastDmgTaken;

        Vector3 targetPos = PlayerController.instance.transform.position;
        if (damageAbsorbed > damageLimit)
        {
            damageAbsorbed = 0;
            float startYrot = reflectSpread * -0.5f;
            float increment = reflectSpread / reflectAmount;
            for (int i = 0; i < reflectAmount; i++)
            {
                float yRot = startYrot + increment * i;
                GameObject currProj = Instantiate(reflectProjectile, _agent.transform.position, _agent.transform.rotation);
                Vector3 angles = currProj.transform.eulerAngles;
                angles.y += yRot;
                currProj.transform.eulerAngles = angles;

                currProj.GetComponent<Projectile>().Init(14.0f, 0.0f, 10.0f, damageAbsorbed, _agent.gameObject, false);
            }
        }
    }
}
