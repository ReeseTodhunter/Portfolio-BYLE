using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDescriptionBox : MonoBehaviour
{
    Canvas canvas;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;



    private void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    public void SetText(string a_title, string a_desc)
    {
        titleText.text = a_title;
        descriptionText.text = a_desc;
    }

    public void Hidden(bool hide)
    {
        if (canvas != null)
        {
            canvas.enabled = !hide;
        }
    }
}
