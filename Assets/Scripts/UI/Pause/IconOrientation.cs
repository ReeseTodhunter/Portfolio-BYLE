using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconOrientation : MonoBehaviour
{
    public int myIconNum;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.rotation = Quaternion.Euler(0.0f,0.0f,0.0f);
    }

    public void clickSelect()
    {
        this.gameObject.GetComponentInParent<PauseMenu>().seletedIcon = myIconNum;
    }
}
