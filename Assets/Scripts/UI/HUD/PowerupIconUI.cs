using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerupIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    PowerupUI controller = null;
    int index = -1;

    public void Initialise(PowerupUI a_controller, int a_index)
    {
        controller = a_controller;
        index = a_index;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (controller == null || index == -1) return;
        controller.SetBox(index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (controller == null || index == -1) return;
        controller.RemoveBox(index);
    }
}
