using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolotovAbility : BaseActivePowerup
{
    [SerializeField] GameObject molotovPrefab;

    public override float UseAbility()
    {
        Instantiate(molotovPrefab, PlayerController.instance.transform.position + (PlayerController.instance.transform.rotation * Vector3.forward * 2.0f), PlayerController.instance.transform.rotation)
            .GetComponent<Projectile>().Init(20, 0, 10, 5, PlayerController.instance.gameObject);

        return base.UseAbility();
    }
}
