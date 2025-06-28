using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPowerUpModifier : BTModifier
{
    public List<GameObject> dropPrefabs;
    public override void Initialise(Character _agent)
    {
        type = modifierType.onDeath;
    }
    public override void ActivateModifier(Character _agent)
    {
        int random = Random.Range(0, dropPrefabs.Count - 1);

        Vector3 rnd = Vector3.zero;
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        rnd = new Vector3(transform.position.x + x, 1, transform.position.z + z);
        Instantiate(dropPrefabs[random], rnd, Quaternion.identity);
    }
}
