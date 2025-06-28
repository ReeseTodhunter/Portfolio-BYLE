using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByleSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject BylePuddle;
    GameObject OriginPuddle;

    Vector3 OriginPos;

    int NumOfPuddles = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnMorePuddles()
    {
        NumOfPuddles = Random.Range(1, 10);

        OriginPuddle = this.transform.GetChild(0).gameObject;

        OriginPos = OriginPuddle.transform.position;

        for (int i = 0; i <= NumOfPuddles; i++ )
        {
            Instantiate(OriginPuddle,new Vector3(OriginPos.x,OriginPos.y, OriginPos.z),new Quaternion(0,0,0,0), this.transform);
        }

    }
}
