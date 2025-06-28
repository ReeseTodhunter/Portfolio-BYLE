using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLogic : MonoBehaviour
{


    public Vector3 endPos;
    public float speed = 0.5f;

    private Vector3 startPos;

    public GameObject LeftDoor;
    public GameObject RightDoor;
    public GameObject DoorCollider;

    public Transform leftEndPos;
    public Transform rightEndPos;
    public Transform closedEndPos;

    public bool doorclosed = false;

    private void Start()
    {
        startPos = transform.position;
    }
    private void Update()
    {

        if (doorclosed == false)
        {
            float dist = Vector3.Distance(RightDoor.transform.position, rightEndPos.position);
            if (dist > 0.1f)
            {

                LeftDoor.transform.position = Vector3.Lerp(LeftDoor.transform.position, leftEndPos.position, speed * Time.deltaTime);
                RightDoor.transform.position = Vector3.Lerp(RightDoor.transform.position, rightEndPos.position, speed * Time.deltaTime);
                DoorCollider.transform.position = new Vector3(DoorCollider.transform.position.x, -5, DoorCollider.transform.position.z);
            }
        }
        else if (doorclosed == true)
        {
            float dist = Vector3.Distance(RightDoor.transform.position, startPos);
            if (dist > 0.1f)
            {
                LeftDoor.transform.position = Vector3.Lerp(LeftDoor.transform.position, closedEndPos.position, speed * Time.deltaTime);
                RightDoor.transform.position = Vector3.Lerp(RightDoor.transform.position, closedEndPos.position, speed * Time.deltaTime);
                DoorCollider.transform.position = new Vector3(DoorCollider.transform.position.x, 5, DoorCollider.transform.position.z);
            }
        }

    }

    public void DoorManagement(bool isRoomLocked)
    {
        gameObject.GetComponent<AudioSource>().volume = GameManager.GMinstance.FXVolume;
        gameObject.GetComponent<AudioSource>().Play();
        doorclosed = isRoomLocked;
    }
}
