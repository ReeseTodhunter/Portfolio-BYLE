using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathVolume : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BTAgent>() != null)
        {
            if(!other.GetComponent<BTAgent>().isAlive()){return;} 
            Destroy(other.gameObject);
        }

    }

}
