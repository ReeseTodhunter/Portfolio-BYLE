using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonExplosionModifier : BTModifier
{
    public GameObject poisonExplosion;
    GameObject[] explosions = new GameObject[8];
    public float explosionSize = 1;

    public override void Initialise(Character _agent)
    {
        title = "Toxic";
        type = modifierType.onDamage;
        base.Initialise(_agent);
    }

    public override void ActivateModifier(Character _agent)
    {
        base.ActivateModifier(_agent);
        Vector3 explosionPos = _agent.transform.position;
        explosionPos.y = 0.5f;
        for (int i = 0; i < 8; i++)
        {
            explosions[i] = Instantiate(poisonExplosion, explosionPos, Quaternion.Euler(0.0f,45.0f*i,0.0f));
            explosions[i].transform.localScale *= explosionSize;
            explosions[i].GetComponent<IProjectile>().Init(2f, -1.0f, 2.0f, 1f, _agent.transform.tag);
        }
    }
}
