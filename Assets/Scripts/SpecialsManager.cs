using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialsManager : MonoBehaviour
{
    //Instance
    public static SpecialsManager instance;

    //Variables to edit
    public int numChicks = 3;
    
    //Stored variables
    public List<GameObject> chicks;



    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this);
            }
        }
        instance = this;
    }
}
