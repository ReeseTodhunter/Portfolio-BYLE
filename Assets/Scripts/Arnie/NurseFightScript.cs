using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseFightScript : MonoBehaviour
{
    public GameObject nursePrefab;
    public GameObject parentAmbulance;

    public bool timerActive = false;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
        }

        if(timer > 4)
        {
            gameObject.GetComponentInParent<RoomController>().shopKeeperPrefab = nursePrefab;
            gameObject.GetComponentInParent<RoomController>().shopKeeperSpawned = true;
            Destroy(parentAmbulance);
            timer = 0;
            timerActive = false;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>() != null)
        {
            CanvasScript.instance.roomClear.clearText.text = "You shouldn't be here";
            CanvasScript.instance.roomClear.roomClear = false;
            CanvasScript.instance.roomClear.Start();
            timerActive = true;
        }
      
    }
    private void OnTriggerExit(Collider other)
    {

        if (other.GetComponent<PlayerController>() != null)
        {
            timerActive = false;
        }
        
    }
}
