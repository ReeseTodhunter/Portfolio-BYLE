using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StealButton : MonoBehaviour
{
    // Singleton instance
    public StealButton instance;

    public bool pressed = false;
    public bool stealMode = false;

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
