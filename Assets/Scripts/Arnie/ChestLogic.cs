using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestLogic : MonoBehaviour
{
    public GameObject Lid;
    public GameObject lidEndRot;
    public GameObject parachute;
    public List<ChestContents> contents;
    public Transform spawnLocation;

    public List<GameObject> weapons;
    public List<GameObject> powerUps;

    public bool interacted = false;
    private bool landed = false;

    public bool openBox = false;

    public bool inZone = false;


    // Update is called once per frame
    void Update()
    {
        OpenChest();
        if(gameObject.transform.position.y < 1.1)
        {
            Destroy(parachute);
            landed = true;
            gameObject.GetComponent<Collider>().enabled = true;
        }

        if (openBox)
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            Lid.transform.eulerAngles = Vector3.Lerp(Lid.transform.eulerAngles, lidEndRot.transform.eulerAngles, 0.05f);
            Lid.transform.position = Vector3.Lerp(Lid.transform.position, lidEndRot.transform.position, 0.05f);
        }



    }

    void Initialise(Transform location)
    {
        spawnLocation = location;

    }

    void OpenChest()
    {
        if (landed)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 1, gameObject.transform.position.z);
            if (GameManager.GMinstance.GetInputDown("keyInteract"))
            {
                if (interacted == false)
                {
                    if (inZone)
                    {
                        openBox = true;

                        System.Random rand = new System.Random();

                        int random = rand.Next(weapons.Count);
                        GameObject itemInstance = Instantiate(weapons[random]);

                        int random2 = rand.Next(powerUps.Count);
                        GameObject powerUp = Instantiate(powerUps[random2]);


                        itemInstance.transform.position = new Vector3(spawnLocation.position.x + Random.Range(-1, 1), spawnLocation.position.y + 2.5f, spawnLocation.position.z + Random.Range(-1, 1));
                        powerUp.transform.position = new Vector3(spawnLocation.position.x + Random.Range(1, -1), spawnLocation.position.y + 1f, spawnLocation.position.z + Random.Range(-0.75f, -1));
                        itemInstance.transform.parent = GameController.instance.roomsParent.transform;
                        powerUp.transform.parent = GameController.instance.roomsParent.transform;
                        powerUp.GetComponent<BasePowerup>().RandomiseModAmount();
                        interacted = true;
                    }
                }

            }
        }
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (!openBox)
            {
                inZone = true;
                CanvasScript.instance.InteractText.text = $"Press '{GameManager.GMinstance.keyInteract}' to open box";
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            inZone = false;
            CanvasScript.instance.InteractText.text = "";
        }

    }
}

[System.Serializable]
public struct ChestContents
{
    public GameObject itemPrefab;
    public GameObject powerUp;
    public float quantity;
}
