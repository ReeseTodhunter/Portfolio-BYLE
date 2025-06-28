using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropModifier : BTModifier
{
    public GameObject dropPrefab;
    public override void Initialise(Character _agent)
    {
        type = modifierType.onDeath;
    }
    public override void ActivateModifier(Character _agent)
    {
        Vector3 rnd = Vector3.zero;
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        rnd = new Vector3(x, 1, z);
        Instantiate(dropPrefab, transform.position + rnd, Quaternion.identity);
    }
}
