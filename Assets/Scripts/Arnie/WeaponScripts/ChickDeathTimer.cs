using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickDeathTimer : MonoBehaviour
{

    public float timer = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 15)
        {
            Destroy(gameObject);
        }
    }
}
