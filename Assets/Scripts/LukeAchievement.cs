using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LukeAchievement : MonoBehaviour
{
    private void OnMouseDown()
    {
        AchievementSystem.UnlockAchievement(36);
        AchievementSystem.Init();
        SaveLoadSystem.instance.SaveAchievements();
        GameManager.GMinstance.UnlockSteamAchievement("Get_Luke'd");
    }
}
