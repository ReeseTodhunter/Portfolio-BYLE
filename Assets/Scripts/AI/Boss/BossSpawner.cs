using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    //This class is used to let bosses spawn, by allowing other classes to inherit its functionality
    // -Tom
    public GameObject bossPrefab; //Boss enemy to spawn
    public virtual void SpawnBoss()
    {
        //Spawn boss
    }
    public virtual void SubscribeBoss(BTAgent _boss)
    {
        //Subscribe boss
    }
    public virtual void UnSubscribeBoss(BTAgent _boss)
    {
        //Boss is dead
    }
}
