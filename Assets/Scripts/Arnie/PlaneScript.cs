using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
    public float timer = 5;
    public float speed = 20;

    public GameObject Chest;
    bool ChestDropped = false;

    public Transform initialPlayerPos;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<AudioSource>().volume = GameManager.GMinstance.FXVolume;
        transform.GetComponent<AudioSource>().Play();
        initialPlayerPos = PlayerController.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + speed * Time.deltaTime);
            CameraController.instance.ShakeCamera(0.2f);
        }
        if(timer < 2.5f & !ChestDropped)
        {
            GameObject chestInstance = Instantiate(Chest);
            chestInstance.transform.position = new Vector3(initialPlayerPos.position.x, initialPlayerPos.position.y + 25, initialPlayerPos.position.z);
            chestInstance.transform.parent = GameController.instance.roomsParent.transform;
            ChestDropped = true;
        }
        if(timer < 0)
        {
            Destroy(this.gameObject);
        }

        timer -= Time.deltaTime;
    }
}
