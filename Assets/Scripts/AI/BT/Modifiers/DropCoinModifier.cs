using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCoinModifier : BTModifier
{
    /*
     * This modifier makes the agent drop bonus coins on death
     * -Tom
     */
    public GameObject coinPrefab;
    [Range (0,1)]
    public float chanceToDrop = 0;
    [Range(0,10)]
    public float amount = 1; 
    public override void Initialise(Character _agent)
    {
    }
    public override void ActivateModifier(Character _agent)
    {
        float chance = Random.Range(0,1);
        if(chance > chanceToDrop){return;}
        for(int i =0; i < amount; i++)
        {
            GameObject coin = Instantiate(coinPrefab,transform.position,Quaternion.identity);
        }
    }
}
