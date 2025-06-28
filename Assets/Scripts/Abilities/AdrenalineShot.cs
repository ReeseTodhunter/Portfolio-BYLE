using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrenalineShot : BaseActivePowerup
{
    [SerializeField] float length = 5.0f; // 
    [SerializeField] float timeScale = 0.5f; // 1 = no difference, 0.5 = half speed, 0.0 = full stop dont do this
    float timer = 0.0f;

    // Areas that this shouldnt effect:
    // • Any timer on the player: animations, cooldowns
    // • Game timer
    // • The timer for this ability
    // • Weapon timers
    //
    // These will need to be hardcoded to not be overridden

    public override float UseAbility()
    {
        timer = length; // Start timer

        // Change time scale
        Time.timeScale *= timeScale;

        // Add speed related modifiers so the player still feels normal
        // Rate of fire does not need to be added since the timers on weapon run independent from time scale changes
        //PlayerController.instance.AddModifier(ModifierType.Speed, timeScale);

        return base.UseAbility();
    }

    private void Update()
    {
        base.Update();

        if (timer > 0.0f)
        {
            timer -= Time.unscaledDeltaTime;
            if (timer <= 0.0f)
            {
                // Reset time scale
                Time.timeScale /= timeScale;

                // Remove player modifiers
                //PlayerController.instance.RemoveModifier(ModifierType.Speed, timeScale);
            }
        }
    }
}
