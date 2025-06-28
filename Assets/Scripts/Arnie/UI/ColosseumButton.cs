using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColosseumButton : MonoBehaviour
{
    public GameObject button;
    private Color color;
    void Start()
    {
        AchievementSystem.Init();
        color = button.GetComponentInChildren<TextMeshProUGUI>().color;
        /*
        if (!AchievementSystem.achievements[6])
        {
            button.GetComponent<Button>().interactable = false;
            Debug.Log("Not ceared level");
            button.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;

        }
        else
        {
            button.GetComponent<Button>().interactable = true;
            button.GetComponentInChildren<TextMeshProUGUI>().color = color;
        }
        */
    }
}
