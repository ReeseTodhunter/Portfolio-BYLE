using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RebindScreen : MonoBehaviour
{
    public List<RebindButton> buttons;

    public void ResetButton()
    {
        GameManager.GMinstance.ResetInputs();
        foreach (RebindButton button in buttons)
        {
            button.UpdateText();
        }
    }
}
