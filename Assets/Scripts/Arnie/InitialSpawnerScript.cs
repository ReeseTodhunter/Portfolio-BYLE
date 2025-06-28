using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSpawnerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Generating two random numbers to add to the position at the start to make it slightly more random
        float integerGenerated1 = Random.Range(-3, 3);


        float integerGenerated2 = Random.Range(-3, 3);


        gameObject.transform.position = new Vector3(gameObject.transform.position.x + integerGenerated1, gameObject.transform.position.y + integerGenerated2, gameObject.transform.position.z);
    }
}
