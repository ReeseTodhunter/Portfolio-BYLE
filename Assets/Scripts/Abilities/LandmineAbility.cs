using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmineAbility : BaseActivePowerup
{
    [SerializeField] float damage;
    [SerializeField] float damageRadius;
    [SerializeField] float armTime;

    [SerializeField] GameObject minePrefab; // Prefab of the landmine
    GameObject mine; // The current landmine that is down

    private void Start()
    {
        mine = null;
    }

    public override float UseAbility()
    {
        // Calculate spawn pos of landmine
        Vector3 landminePosition = PlayerController.instance.transform.position - Vector3.up;

        // Spawn landmine
        if (mine == null)
        {
            mine = Instantiate(minePrefab, landminePosition, Quaternion.identity);
        }
        else
        {
            mine.transform.position = landminePosition;
        }

        // Play sound effect
        GetComponent<AudioSource>().Play();

        // Initialise landmine
        if (mine.GetComponent<LandmineObject>() != null) mine.GetComponent<LandmineObject>().SetLandmine(this, damage, damageRadius, armTime);

        // Cooldown only happens when the landmine explodes, handled on the landmine object
        return 0.0f;
    }
}
