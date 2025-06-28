using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class RebindButton : MonoBehaviour
{
    public TextMeshProUGUI text;
    public string keybind;

    bool listening = false; // If true then the next key pressed will be bound to whatever action this button corresponds to
    KeyCode currentKey = KeyCode.None;

    public void OnClick()
    {
        listening = true;
        text.text = ">     <";
    }

    public void UpdateText()
    {
        currentKey = (KeyCode)PlayerPrefs.GetInt(keybind);
        text.text = currentKey.ToString();
    }

    private void Start()
    {
        UpdateText();
    }

    private void OnGUI()
    {
        if (listening)
        {
            if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.MouseDown)
            {
                // Get pressed key
                currentKey = Event.current.keyCode;
                if (currentKey == KeyCode.None) currentKey = Event.current.button + KeyCode.Mouse0;

                text.text = currentKey.ToString(); // Update text

                GameManager.GMinstance.RebindInput(keybind, currentKey); // Rebind key
                listening = false; // Stop listening for inputs
            }
        }
    }
}
