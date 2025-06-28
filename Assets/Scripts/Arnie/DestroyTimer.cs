using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    float timer = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer - Time.deltaTime;
        if(timer < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
