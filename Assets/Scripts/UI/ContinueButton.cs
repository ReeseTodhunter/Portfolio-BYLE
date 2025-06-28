using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContinueButton : MonoBehaviour
{

    public GameObject button;
    private Color color;
    void Start()
    {
        color = button.GetComponentInChildren<TextMeshProUGUI>().color;
   
        if(SaveLoadSystem.instance.LoadGame() == null)
        {
            button.GetComponent<Button>().interactable = false;
            Debug.Log("No Save file");
            button.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;

        }
        else
        {
            button.GetComponent<Button>().interactable = true;
            button.GetComponentInChildren<TextMeshProUGUI>().color = color;
        }
    }
}
