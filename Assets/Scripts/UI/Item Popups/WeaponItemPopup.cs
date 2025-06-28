using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemPopup : MonoBehaviour
{
    BaseWeapon parent;

    private void Start()
    {
        parent = transform.parent.GetComponent<BaseWeapon>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerController.instance.gameObject && parent != null)
        {
            parent.SpawnDescBox();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.instance.gameObject && parent != null)
        {
            parent.RemoveDescBox();
        }
    }
}
