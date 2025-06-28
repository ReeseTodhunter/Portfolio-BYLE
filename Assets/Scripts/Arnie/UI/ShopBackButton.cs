using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBackButton : MonoBehaviour
{
    // Singleton instance
    public static ShopBackButton instance;

    public bool pressed = false;

    public void Awake()
    {
        // Initialise Singleton
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
    }


    public void OnPressTheButton()
    {
        pressed = true;
    }
}
