using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class BTAchievementOnDeath : BTModifier
{
    public int achievementIndex = 0;
    public override void Initialise(Character _agent)
    {
        type = modifierType.onDeath;
    }
    public override void ActivateModifier(Character _agent)
    {
        if (SteamManager.Initialized)
        {
            switch (achievementIndex)
            {
                case 1:
                    Steamworks.SteamUserStats.GetAchievement("Orphaned", out bool achievementCompleted1);

                    if (!achievementCompleted1)
                    {
                        SteamUserStats.SetAchievement("Orphaned");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 2:
                    Steamworks.SteamUserStats.GetAchievement("Extreme_Dietary_Tactics", out bool achievementCompleted2);

                    if (!achievementCompleted2)
                    {
                        SteamUserStats.SetAchievement("Extreme_Dietary_Tactics");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 4:
                    Steamworks.SteamUserStats.GetAchievement("Duelist", out bool achievementCompleted3);

                    if (!achievementCompleted3)
                    {
                        SteamUserStats.SetAchievement("Duelist");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 5:
                    Steamworks.SteamUserStats.GetAchievement("VIP_Slayer", out bool achievementCompleted4);

                    if (!achievementCompleted4)
                    {
                        SteamUserStats.SetAchievement("VIP_Slayer");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 8:
                    Steamworks.SteamUserStats.GetAchievement("Market_Crash", out bool achievementCompleted5);

                    if (!achievementCompleted5)
                    {
                        SteamUserStats.SetAchievement("Market_Crash");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 9:
                    Steamworks.SteamUserStats.GetAchievement("You_Never_Learn", out bool achievementCompleted6);

                    if (!achievementCompleted6)
                    {
                        SteamUserStats.SetAchievement("You_Never_Learn");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 20:
                    Steamworks.SteamUserStats.GetAchievement("Hippocratic_Oath", out bool achievementCompleted7);

                    if (!achievementCompleted7)
                    {
                        SteamUserStats.SetAchievement("Hippocratic_Oath");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 21:
                    Steamworks.SteamUserStats.GetAchievement("Shiny_Hunter", out bool achievementCompleted8);

                    if (!achievementCompleted8)
                    {
                        SteamUserStats.SetAchievement("Shiny_Hunter");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 28:
                    Steamworks.SteamUserStats.GetAchievement("Hell_On_Earth", out bool achievementCompleted9);

                    if (!achievementCompleted9)
                    {
                        SteamUserStats.SetAchievement("Hell_On_Earth");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 38:
                    Steamworks.SteamUserStats.GetAchievement("Boris_Begone", out bool achievementCompleted10);

                    if (!achievementCompleted10)
                    {
                        SteamUserStats.SetAchievement("Boris_Begone");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 40:
                    Steamworks.SteamUserStats.GetAchievement("End_The_Cult", out bool achievementCompleted11);

                    if (!achievementCompleted11)
                    {
                        SteamUserStats.SetAchievement("End_The_Cult");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 41:
                    Steamworks.SteamUserStats.GetAchievement("Jugger-not", out bool achievementCompleted12);

                    if (!achievementCompleted12)
                    {
                        SteamUserStats.SetAchievement("Jugger-not");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 42:
                    Steamworks.SteamUserStats.GetAchievement("Fireman", out bool achievementCompleted13);

                    if (!achievementCompleted13)
                    {
                        SteamUserStats.SetAchievement("Fireman");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 43:
                    Steamworks.SteamUserStats.GetAchievement("Armageddon", out bool achievementCompleted14);

                    if (!achievementCompleted14)
                    {
                        SteamUserStats.SetAchievement("Armageddon");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 44:
                    Steamworks.SteamUserStats.GetAchievement("Poor_Mans_Magic", out bool achievementCompleted15);

                    if (!achievementCompleted15)
                    {
                        SteamUserStats.SetAchievement("Poor_Mans_Magic");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 45:
                    Steamworks.SteamUserStats.GetAchievement("The_Whopper", out bool achievementCompleted16);

                    if (!achievementCompleted16)
                    {
                        SteamUserStats.SetAchievement("The_Whopper");
                        SteamUserStats.StoreStats();
                    }
                    break;
                case 46:
                    Steamworks.SteamUserStats.GetAchievement("Pile_Up", out bool achievementCompleted17);

                    if (!achievementCompleted17)
                    {
                        SteamUserStats.SetAchievement("Pile_Up");
                        SteamUserStats.StoreStats();
                    }
                    break;
            }
        }
        
        AchievementSystem.UnlockAchievement(achievementIndex);
        AchievementSystem.Init();
        SaveLoadSystem.instance.SaveAchievements();
    }
}
