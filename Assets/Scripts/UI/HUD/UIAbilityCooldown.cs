using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAbilityCooldown : MonoBehaviour
{
    [SerializeField] Image abilityBackground = null;
    [SerializeField] Image abilityIcon = null;
    [SerializeField] GameObject abilityTimer = null;
    [SerializeField] Text abilityTimerText = null;

    private void Update()
    {
        if (PlayerController.instance.GetAbility() != null)
        {
            // Player has an ability equipped
            abilityIcon.enabled = true;
            abilityIcon.sprite = PlayerController.instance.GetAbility().GetSprite();

            float sliderValue = Mathf.Max(0.0f, PlayerController.instance.GetAbilityCooldownTime() / PlayerController.instance.GetAbility().GetCooldownTime());
            GetComponent<Slider>().value = sliderValue; // Set cooldown bar value
            if (sliderValue <= 0.0f)
            {
                // Ability is off cooldown
                abilityTimer.SetActive(false);
                abilityBackground.color = Color.white;
                abilityIcon.color = Color.white;
            }
            else
            {
                // Ability is under cooldown
                // Set icon timer text
                abilityTimer.SetActive(true);
                abilityTimerText.text = (Mathf.Round(PlayerController.instance.GetAbilityCooldownTime() + 0.5f)).ToString();

                // Gray out icon
                abilityBackground.color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
                abilityIcon.color = new Color(0.7f, 0.7f, 0.7f, 1.0f); ;

                // Play use animation
                abilityIcon.transform.localScale = Vector3.one * Mathf.Max(1.0f, 1.5f + 2.2f*(PlayerController.instance.GetAbilityCooldownTime() - PlayerController.instance.GetAbility().GetCooldownTime()));
            }
        }
        else
        {
            // Player has no ability equipped
            abilityIcon.enabled = false;
        }
    }
}
