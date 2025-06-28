using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerScript : MonoBehaviour
{
    // Item Pools
    public List<GameObject> potentialItems;
    public List<GameObject> specialItems;
    public bool weaponDealer = false;

    public List<GameObject> spawnedItems;

    public List<Transform> itemPositions;

    public GameObject itemParent;

    private GameObject smokeObject;

    // Start is called before the first frame update
    void Start()
    {
        smokeObject = Resources.Load("VFX/Ruku/Teleport") as GameObject;
        GameObject smoke = GameObject.Instantiate(smokeObject, transform.position, Quaternion.identity);
        smoke.transform.localScale = new Vector3(3, 3, 3);

        for (int i = 0; i < itemPositions.Count; i++)
        {

            if (weaponDealer)
            {
                if (i != 2)
                {
                    System.Random random = new System.Random();
                    int rand = random.Next(potentialItems.Count);

                    GameObject instantiatedItem = Instantiate(potentialItems[rand]);
                    instantiatedItem.transform.position = itemPositions[i].position;
                    instantiatedItem.transform.eulerAngles = new Vector3(0, 0, 0);
                    instantiatedItem.transform.SetParent(itemParent.transform, true);

                    potentialItems.Remove(potentialItems[rand]);
                }
                else
                {
                    System.Random random = new System.Random();
                    

                    int randChance = random.Next(3);

                    if (randChance == 2)
                    {
                        int rand = random.Next(specialItems.Count);

                        GameObject instantiatedItem = Instantiate(specialItems[rand]);
                        instantiatedItem.transform.position = itemPositions[i].position;
                        instantiatedItem.transform.eulerAngles = new Vector3(0, 0, 0);
                        instantiatedItem.transform.SetParent(itemParent.transform, true);

                        potentialItems.Remove(specialItems[rand]);
                    }
                    else
                    {
                        int rand = random.Next(potentialItems.Count);

                        GameObject instantiatedItem = Instantiate(potentialItems[rand]);
                        instantiatedItem.transform.position = itemPositions[i].position;
                        instantiatedItem.transform.eulerAngles = new Vector3(0, 0, 0);
                        instantiatedItem.transform.SetParent(itemParent.transform, true);

                        potentialItems.Remove(potentialItems[rand]);
                    }
                }
                    
            }
            else
            {
                System.Random random = new System.Random();
                int rand = random.Next(potentialItems.Count);

                GameObject instantiatedItem = Instantiate(potentialItems[rand]);
                instantiatedItem.transform.position = itemPositions[i].position;
                instantiatedItem.transform.eulerAngles = new Vector3(0, 0, 0);
                instantiatedItem.transform.SetParent(itemParent.transform, true);

                potentialItems.Remove(potentialItems[rand]);
            }
            
            

            
        }

        foreach(Transform item in itemParent.transform)
        {
            spawnedItems.Add(item.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < spawnedItems.Count; i++)
        {
            if (spawnedItems[i] == null || spawnedItems[i].transform.parent != itemParent.transform)
            {
                ShopDestroy();
            }
        }
    }

    public void ShopDestroy()
    {
        smokeObject = Resources.Load("VFX/Ruku/Teleport") as GameObject;
        GameObject smoke = GameObject.Instantiate(smokeObject, transform.position, Quaternion.identity);
        smoke.transform.localScale = new Vector3(3, 3, 3);

        ItemPopupController.instance.ClearBox();
        ItemPopupController.instance.active = false;


        object[] obj = GameObject.FindSceneObjectsOfType(typeof(GameObject));
        foreach (object o in obj)
        {
            GameObject g = (GameObject)o;
            if(g.GetComponent<BaseWeapon>() != null)
            {
                if(g.GetComponent<BaseWeapon>().currState == BaseWeapon.WeaponState.DROPPED)
                {
                    Destroy(g);
                }
                
            }
            if (g.GetComponent<BasePowerup>() != null)
            {
                Destroy(g);
            }
        }

        Destroy(this.gameObject);
        ItemPopupController.instance.active = true;
    }
}
