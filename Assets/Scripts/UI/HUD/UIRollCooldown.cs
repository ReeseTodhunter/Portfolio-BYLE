using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRollCooldown : MonoBehaviour
{
    Slider slider;
    //Image icon;
    public Slider rollChargeBoxSlider; // defined in editor
    public Text rollCharges; // defined in editor
    bool active = false;

    //float animationTimer = 0.0f;

    private void Start()
    {
        if (PlayerController.instance != null) active = true;
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (active)
        {
            if (PlayerController.instance.GetRollCharges() == 0)
            {
                slider.value = PlayerController.instance.GetRollCooldown();
                rollChargeBoxSlider.gameObject.SetActive(false);
                rollChargeBoxSlider.value = 1.0f;
            }
            else if (PlayerController.instance.GetModifier(ModifierType.RollCharges) > 0)
            {
                slider.value = 0.0f;
                rollChargeBoxSlider.gameObject.SetActive(true);
                rollChargeBoxSlider.value = PlayerController.instance.GetRollCooldown();
            }
            rollCharges.text = PlayerController.instance.GetRollCharges().ToString();
        }
    }
}
