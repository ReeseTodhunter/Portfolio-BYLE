using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSize : MonoBehaviour
{
    //public GameObject model;
    // Start is called before the first frame update
    void Start()
    {
        float random = Random.Range(0, 1.2f);
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + random, gameObject.transform.localScale.y + random, gameObject.transform.localScale.z + random);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
