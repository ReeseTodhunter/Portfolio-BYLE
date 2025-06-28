using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    Slider slider;
    Text text;

    bool active = false;

    private void Start()
    {
        if (PlayerController.instance != null) active = true;
        slider = GetComponent<Slider>();
        text = GetComponentInChildren<Text>(false);
    }

    private void Update()
    {
        if (active)
        {
            slider.value = PlayerController.instance.GetHealthRatio();
            text.text = Mathf.Max(0, (int)PlayerController.instance.GetHealth()).ToString() + " / " + ((int)PlayerController.instance.GetMaxHealth()).ToString();
        }
    }
}
