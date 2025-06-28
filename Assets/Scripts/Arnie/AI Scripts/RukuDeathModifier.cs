using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RukuDeathModifier : BTModifier
{
    public override void Initialise(Character _agent)
    {
        type = modifierType.onDeath;
    }
    public override void ActivateModifier(Character _agent)
    {
        if (PlayerController.instance.primaryWeapon.GetComponent<KatanaWeapon>() != null)
        {
            AchievementSystem.UnlockAchievement(54);
            AchievementSystem.Init();
            SaveLoadSystem.instance.SaveAchievements();
            GameManager.GMinstance.UnlockSteamAchievement("Swordfight");
        }

    }
}
