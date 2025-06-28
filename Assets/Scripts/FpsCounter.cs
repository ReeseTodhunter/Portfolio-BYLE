using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FpsCounter : MonoBehaviour
{
    private int frameCounter = 0;
    private float timer = 0;
    void Update()
    {
        //iterate
        timer += Time.deltaTime;
        frameCounter ++;
        if(timer < 1){return;}
        timer = 0;
        this.GetComponent<TextMeshProUGUI>().text = "FPS: " + frameCounter; 
        frameCounter = 0;
    }
}
