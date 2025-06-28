using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainWeapon : MonoBehaviour
{
    Sprite curentMainWeapon = null;
    bool active = true;

    void Start()
    {
        if (PlayerController.instance == null) active = false;
        if (gameObject.GetComponent<Image>() == null) active = false;
    }

    void Update()
    {
        if (active)
        {
            curentMainWeapon = PlayerController.instance.GetMainWeaponSprite();
            if (curentMainWeapon == null)
            {
                gameObject.GetComponent<Image>().enabled = false;
            }
            else
            {
                gameObject.GetComponent<Image>().enabled = true;
                gameObject.GetComponent<Image>().sprite = curentMainWeapon;
            }
        }
    }
}
