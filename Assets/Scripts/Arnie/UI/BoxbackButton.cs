using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxbackButton : MonoBehaviour
{
    public bool pressed = false;



    public void OnPressTheButton()
    {
        pressed = true;
    }
}
