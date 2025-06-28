using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using Steamworks;

public class SaveLoadSystem : MonoBehaviour
{
    static public SaveLoadSystem instance;

    string filePath;
    string achievePath;
    string dataPath;

    bool[] weaponsPickedUp;
    bool[] enemiesEncountered;

    private void Awake()
    {
        filePath = Application.persistentDataPath + "/save.dat";

        achievePath = Application.persistentDataPath + "/achievements.dat";

        dataPath = Application.persistentDataPath + "/intermediateData.dat";

        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this);
            }
        }
        instance = this;
    }

    public void SaveGame(SaveData saveData)
    {
        FileStream dataStream;
        if (File.Exists(filePath))
        {
            dataStream = File.OpenWrite(filePath);
        }
        else
        {
            Debug.Log("File did not exist so I made it");
            dataStream = File.Create(filePath);
        }
        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, saveData);

        dataStream.Close();
        

        
    }

    public void SaveAchievements()
    {
        FileStream dataStream;
        bool[] achieveData = AchievementSystem.achievements;
        if (File.Exists(achievePath))
        {
            dataStream = File.OpenWrite(achievePath);
        }
        else
        {
            dataStream = File.Create(achievePath);
        }
        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, achieveData);
        dataStream.Close();
    }

    public void SaveAchievementData(AchievementData achievementData)
    {
        FileStream dataStream;
        if (File.Exists(dataPath))
        {
            dataStream = File.OpenWrite(dataPath);
        }
        else
        {
            Debug.Log("File did not exist so I made it");
            dataStream = File.Create(dataPath);
        }
        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, achievementData);

        dataStream.Close();
    }
    public SaveData LoadGame()
    {
        if(File.Exists(filePath))
        {
            FileStream dataStream = File.OpenRead(filePath);

            BinaryFormatter converter = new BinaryFormatter();
            SaveData saveData = (SaveData)converter.Deserialize(dataStream);
            Debug.Log(saveData.name +" " + saveData.levelSeed + " " + saveData.score);
            dataStream.Close();
            Debug.Log("Attempted to Load game");
            return saveData;
        }
        else
        {
            Debug.Log("Save File not found" + filePath);
            // Make a new save file
            return null;
        }
    }
    public void LoadAchievements()
    {
        if (File.Exists(achievePath))
        {
            FileStream dataStream = File.OpenRead(achievePath);

            BinaryFormatter converter = new BinaryFormatter();
            bool[] achievements = (bool[])converter.Deserialize(dataStream);
            AchievementSystem.achievements = achievements;
            dataStream.Close();
            //Debug.Log("Attempted to Load Achievements");
            return;
        }
        else
        {
            Debug.Log("Save File not found" + achievePath);
            // Make a new save file
            return;
        }
    }

    public AchievementData LoadAchievementData()
    {
        for (int i = 0; i < AchievementSystem.achievementInformation.Length; i++)
        {
            #region CheckSteamAchievements
            if (AchievementSystem.achievementInformation[i].Obtained == true)
            {
                switch (i)
                {
                    case 0:
                        UnlockSteamAchievement("Straight_Outta_Basic");
                        break;
                    case 1:
                        UnlockSteamAchievement("Orphaned");
                        break;
                    case 2:
                        UnlockSteamAchievement("Extreme_Dietary_Tactics");
                        break;
                    case 3:
                        UnlockSteamAchievement("Worthy");
                        break;
                    case 4:
                        UnlockSteamAchievement("Duelist");
                        break;
                    case 5:
                        UnlockSteamAchievement("VIP_Slayer");
                        break;
                    case 6:
                        UnlockSteamAchievement("Explorer");
                        break;
                    case 7:
                        UnlockSteamAchievement("Shopping_Spree");
                        break;
                    case 8:
                        UnlockSteamAchievement("Market_Crash");
                        break;
                    case 9:
                        UnlockSteamAchievement("You_Never_Learn");
                        break;
                    case 10:
                        UnlockSteamAchievement("Mysterious_Box");
                        break;
                    case 11:
                        UnlockSteamAchievement("Future_Star");
                        break;
                    case 12:
                        UnlockSteamAchievement("Welcome_To_The_Top");
                        break;
                    case 13:
                        UnlockSteamAchievement("Certified_Danger");
                        break;
                    case 14:
                        UnlockSteamAchievement("Classified_Lethal_Weapon");
                        break;
                    case 15:
                        UnlockSteamAchievement("This_Is_Getting_Crazy");
                        break;
                    case 16:
                        UnlockSteamAchievement("Untouchable");
                        break;
                    case 17:
                        UnlockSteamAchievement("Part-time_Ghost");
                        break;
                    case 18:
                        UnlockSteamAchievement("The_Matrix");
                        break;
                    case 19:
                        UnlockSteamAchievement("Cheaters_Always_Win");
                        break;
                    case 20:
                        UnlockSteamAchievement("Hippocratic_Oath");
                        break;
                    case 21:
                        UnlockSteamAchievement("Shiny_Hunter");
                        break;
                    case 22:
                        UnlockSteamAchievement("Look_I_Got_The_Whole_Set");
                        break;
                    case 23:
                        UnlockSteamAchievement("Fantastical_Warfare");
                        break;
                    case 24:
                        UnlockSteamAchievement("Man_At_Arms");
                        break;
                    case 25:
                        UnlockSteamAchievement("Penny_Pincher");
                        break;
                    case 26:
                        UnlockSteamAchievement("Stonks");
                        break;
                    case 27:
                        UnlockSteamAchievement("Byllionaire");
                        break;
                    case 28:
                        UnlockSteamAchievement("Hell_On_Earth");
                        break;
                    case 29:
                        UnlockSteamAchievement("Mike_Main");
                        break;
                    case 30:
                        UnlockSteamAchievement("Rambo_Realist");
                        break;
                    case 31:
                        UnlockSteamAchievement("Vlad_Visionary");
                        break;
                    case 32:
                        UnlockSteamAchievement("One_Eye_Optimist");
                        break;
                    case 33:
                        UnlockSteamAchievement("Dart_Director");
                        break;
                    case 34:
                        UnlockSteamAchievement("Pure_Commitment");
                        break;
                    case 35:
                        UnlockSteamAchievement("Narrow_Escape");
                        break;
                    case 36:
                        UnlockSteamAchievement("Get_Luke'd");
                        break;
                    case 37:
                        UnlockSteamAchievement("Byle_Super_Fan");
                        break;
                    case 38:
                        UnlockSteamAchievement("Boris_Begone");
                        break;
                    case 39:
                        UnlockSteamAchievement("Pistol_Connoisseur");
                        break;
                    case 40:
                        UnlockSteamAchievement("End_The_Cult");
                        break;
                    case 41:
                        UnlockSteamAchievement("Jugger-not");
                        break;
                    case 42:
                        UnlockSteamAchievement("Fireman");
                        break;
                    case 43:
                        UnlockSteamAchievement("Armageddon");
                        break;
                    case 44:
                        UnlockSteamAchievement("Poor_Mans_Magic");
                        break;
                    case 45:
                        UnlockSteamAchievement("The_Whopper");
                        break;
                    case 46:
                        UnlockSteamAchievement("Pile_Up");
                        break;
                    case 47:
                        UnlockSteamAchievement("Nice");
                        break;
                    case 48:
                        UnlockSteamAchievement("Karlos_Conquest");
                        break;
                    case 49:
                        UnlockSteamAchievement("Byleaholic");
                        break;
                    case 50:
                        UnlockSteamAchievement("Xerxes_Extremist");
                        break;
                    case 51:
                        UnlockSteamAchievement("Surfing_Spirit");
                        break;
                    case 52:
                        UnlockSteamAchievement("Wave_Phantom");
                        break;
                    case 53:
                        UnlockSteamAchievement("Tsunami_Ghost");
                        break;
                    case 54:
                        UnlockSteamAchievement("Swordfight");
                        break;
                    case 55:
                        UnlockSteamAchievement("Speedy_Gonzales");
                        break;
                    case 56:
                        UnlockSteamAchievement("Power_House");
                        break;
                    case 57:
                        UnlockSteamAchievement("Brick_Wall");
                        break;
                    case 58:
                        UnlockSteamAchievement("Die_Hard");
                        break;
                    case 59:
                        break;
                    case 60:
                        break;
                    default:
                        break;
                }
            }
            #endregion
        }

        if (File.Exists(dataPath))
        {
            FileStream dataStream = File.OpenRead(dataPath);

            BinaryFormatter converter = new BinaryFormatter();
            AchievementData achievementData = (AchievementData)converter.Deserialize(dataStream);
            dataStream.Close();
            Debug.Log("Attempted to Load Achievement Data");


            //Check the loaded data
            Debug.Log("Loaded data");
            for (int i = 0; i < achievementData.weaponsPickedUp.Length; i++)
            {
                Debug.Log("Weapon: " + i + ": " + achievementData.weaponsPickedUp[i]);
            }
            for (int i = 0; i < achievementData.enemiesEncountered.Length; i++)
            {
                Debug.Log("Enemy " + i + ": " + achievementData.enemiesEncountered[i]);
            }

            int weaponsTemp = GameManager.GMinstance.weapons.Count;
            int enemiesTemp = GameManager.GMinstance.enemiesEncountered.Length;
            weaponsPickedUp = new bool[weaponsTemp];
            enemiesEncountered = new bool[enemiesTemp];

            //Will only go off when new weapons are added for the first time
            if (weaponsPickedUp.Length > achievementData.weaponsPickedUp.Length)
            {
                for (int ass = 0; ass < achievementData.weaponsPickedUp.Length; ass++)
                {
                    weaponsPickedUp[ass] = achievementData.weaponsPickedUp[ass];
                }
                achievementData.weaponsPickedUp = new bool[weaponsPickedUp.Length];
                achievementData.weaponsPickedUp = weaponsPickedUp;
            }

            //Ensures the encountered enemies array is kept big enough
            if (enemiesEncountered.Length > achievementData.enemiesEncountered.Length)
            {
                for (int ass = 0; ass < achievementData.enemiesEncountered.Length; ass++)
                {
                    enemiesEncountered[ass] = achievementData.enemiesEncountered[ass];
                }
                achievementData.enemiesEncountered = new bool[enemiesEncountered.Length];
                achievementData.enemiesEncountered = enemiesEncountered;
            }

            //Check the Resized data
            Debug.Log("Resized Data");
            for (int i = 0; i < achievementData.weaponsPickedUp.Length; i++)
            {
                Debug.Log("Weapon: " + i + ": " + achievementData.weaponsPickedUp[i]);
            }
            for (int i = 0; i < achievementData.enemiesEncountered.Length; i++)
            {
                Debug.Log("Enemy " + i + ": " + achievementData.enemiesEncountered[i]);
            }

            return achievementData;
        }
        else
        {
            
            // Make a new save file
            return null;
        }
    }

    public void ResetSave()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Deleted Save Game");
        }
    }
    public void ResetData()
    {
        
        if(File.Exists(dataPath))
        {
            File.Delete(dataPath);
            Debug.Log("Deleted Data");
        }
    }

    public void ResetAchievements()
    {
        if (File.Exists(achievePath))
        {
            File.Delete(achievePath);
            AchievementSystem.achievements = new bool[AchievementSystem.achievements.Length];
            Debug.Log("Deleted Achievements");
        }
    }

    public void UnlockSteamAchievement(string nameOfAchievementStoredOnSteam)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement(nameOfAchievementStoredOnSteam, out bool steamAchievement);

            if (!steamAchievement)
            {
                SteamUserStats.SetAchievement(nameOfAchievementStoredOnSteam);
                SteamUserStats.StoreStats();
            }
        }
    }
}
