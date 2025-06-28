using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeModifier : BTModifier
{
    /*
     * This modifier makes the agent explode on death, damaging nearby enemies and the player
     * -Tom
     */

    public GameObject explosionPrefab; //explosion effect prefab
    public float agentDamage = 10; //damage to other agents
    public float playerDamage = 30; //damage to player
    public float damageRadius = 1; //radius of damage
    public bool showBlasRadius = false; //blast radius debug
    public float chanceToDelayExplode = 0.2f; //chance for the explosion to be delayed
    public float explosionSize = 1; //size of explosion effect
    public float camShakeStrength = 2; //camera shake strength
    public override void Initialise(Character _agent) //Decides when to explode
    {
        if(chanceToDelayExplode > 0 && Random.Range(0f, 1f) <= chanceToDelayExplode)
        {
            type = modifierType.onDissapear;
            return;
        }
        else
        {
            type = modifierType.onDeath;
        }
    }
    public override void ActivateModifier(Character _agent)
    {
        if(type == modifierType.onDeath)
        {
            if(_agent.TryGetComponent<BTAgent>(out BTAgent agent))
            {
                agent.SetDeathDuration(0.1f); //hide the agent
            }
        }
        base.ActivateModifier(_agent);
        Vector3 explosionPos = _agent.transform.position;
        explosionPos.y = 0.5f;
        GameObject explosion = Instantiate(explosionPrefab, explosionPos,Quaternion.identity); //Instantiate explosion
        explosion.transform.localScale *= explosionSize;
        Collider[] colliders = Physics.OverlapSphere(_agent.transform.position, damageRadius);
        foreach(Collider col in colliders) //Get all nearby agents and the player, then damage them
        {
            if(col.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.Damage(playerDamage);
            }
            else if(col.gameObject.TryGetComponent<BTAgent>(out BTAgent agent))
            {
                agent.Damage(agentDamage);
            }
        }
        CameraController.instance.ShakeCameraOverTime(0.5f, camShakeStrength); //shake camera
        return;
    }
    public void OnDrawGizmos() //show blast radius
    {
        if(!showBlasRadius){return;}
        Vector3 pos = transform.position;
        pos.y = 0.5f;
        Gizmos.DrawWireSphere(pos,damageRadius);
    }
}
