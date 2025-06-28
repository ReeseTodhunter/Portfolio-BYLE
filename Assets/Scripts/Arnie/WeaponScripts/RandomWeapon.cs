using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWeapon : MonoBehaviour
{
    public List<GameObject> potentialBoxItems;
    public GameObject itemPosition;

    public GameObject weaponParent;
    // Start is called before the first frame update
    void Start()
    {
        weaponParent = GameController.instance.weaponParent;
        int rand = Random.Range(0, potentialBoxItems.Count);
        GameObject instantiatedItem = Instantiate(potentialBoxItems[rand]);
        instantiatedItem.transform.position = itemPosition.transform.position;
        instantiatedItem.transform.eulerAngles = itemPosition.transform.eulerAngles;

        if(weaponParent.transform.childCount > 0)
        {
            foreach (Transform child in weaponParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            instantiatedItem.transform.parent = weaponParent.transform;
        }
        else
        {
            instantiatedItem.transform.parent = weaponParent.transform;
        }
    }

}
