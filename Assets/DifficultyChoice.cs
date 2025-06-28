using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyChoice : MonoBehaviour
{
    public Toggle Easy;
    public Toggle Medium;
    public Toggle Hard;

    public int difficultyChoice;
    // Start is called before the first frame update
    void Start()
    {
        if(difficultyChoice == 3)
        {
            Hard.isOn = true;
        }
        else if(difficultyChoice == 1)
        {
            Medium.isOn = true;
        }
        else
        {
            Easy.isOn = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Easy.isOn)
        {
            if (difficultyChoice != 0)
            { PlayerPrefs.SetInt("difficulty", 0); }
            difficultyChoice = 0;
            
        }
        if (Medium.isOn)
        {
            if (difficultyChoice != 1)
            { PlayerPrefs.SetInt("difficulty", 1); }
            difficultyChoice = 3;
        }
        if(Hard.isOn)
        {
            if (difficultyChoice != 3)
            { PlayerPrefs.SetInt("difficulty", 3); }
            PlayerPrefs.SetInt("difficulty", 3);
            difficultyChoice = 3;
        }
    }
}
