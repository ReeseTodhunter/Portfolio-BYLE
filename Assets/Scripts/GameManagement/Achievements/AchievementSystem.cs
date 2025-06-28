using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*

        // TO ACTIVATE A ACHIVEMENT USE THIS CODE AND PASS IN THE CORRESPONDING INDEX
        AchievementSystem.UnlockAchievement(6);
        AchievementSystem.Init();
        SaveLoadSystem.instance.SaveAchievements();

*/


public static class AchievementSystem
{
    public static bool[] achievements = new bool[64];
    //public static List<Achievement> achievementInformation = new List<Achievement>();
    public static Achievement[] achievementInformation = new Achievement[64];
    public static void Init()
    {
        SaveLoadSystem.instance.LoadAchievements();
        
        // Initialises achievements

        achievementInformation[0] = new Achievement(achievements[0],
                                                    "Straight outta basic",
                                                    "Complete the tutorial",
                                                    Resources.Load<Sprite>("Icons/" + 0));  // STEAM IMPLEMENTED

        achievementInformation[1] = new Achievement(achievements[1],
                                                    "Orphaned",
                                                    "Defeat the mother slime",
                                                    Resources.Load<Sprite>("Icons/" + 1));  // STEAM IMPLEMENTED

        achievementInformation[2] = new Achievement(achievements[2],
                                                    "Extreme dietery tactics",
                                                    "Find and defeat Dave",
                                                    Resources.Load<Sprite>("Icons/" + 2));  // STEAM IMPLEMENTED

        achievementInformation[3] = new Achievement(achievements[3],
                                                    "Worthy",
                                                    "Prove your worth to a totem",
                                                    Resources.Load<Sprite>("Icons/" + 3));  // STEAM IMPLEMENTED

        achievementInformation[4] = new Achievement(achievements[4],
                                                    "Duelist",
                                                    "Find and Defeat Ruku",
                                                    Resources.Load<Sprite>("Icons/" + 4));  // STEAM IMPLEMENTED

        achievementInformation[5] = new Achievement(achievements[5],
                                                    "VIP Slayer",
                                                    "Defeat an elite enemy",
                                                    Resources.Load<Sprite>("Icons/" + 5));  // STEAM IMPLEMENTED

        achievementInformation[6] = new Achievement(achievements[6],
                                                    "Explorer",
                                                    "Clear an entire level",
                                                    Resources.Load<Sprite>("Icons/" + 6));  // STEAM IMPLEMENTED

        achievementInformation[7] = new Achievement(achievements[7],
                                                    "Shopping Spree",
                                                    "Obtain an item from the shop by any means",
                                                    Resources.Load<Sprite>("Icons/" + 7));  // STEAM IMPLEMENTED

        achievementInformation[8] = new Achievement(achievements[8],
                                                    "Market Crash",
                                                    "Kill the shopkeeper",
                                                    Resources.Load<Sprite>("Icons/" + 8));  // STEAM IMPLEMENTED

        achievementInformation[9] = new Achievement(achievements[9],
                                                    "You never learn",
                                                    "Kill the shopkeeper (Final Form)",
                                                    Resources.Load<Sprite>("Icons/" + 9));  // STEAM IMPLEMENTED

        achievementInformation[10] = new Achievement(achievements[10],
                                                    "Mysterious box",
                                                    "Open a mystery box",
                                                    Resources.Load<Sprite>("Icons/" + 10));  // STEAM IMPLEMENTED

        achievementInformation[11] = new Achievement(achievements[11],
                                                    "Future star",
                                                    "Score a goal",
                                                    Resources.Load<Sprite>("Icons/" + 11));  // STEAM IMPLEMENTED

        achievementInformation[12] = new Achievement(achievements[12],
                                                    "Welcome to the top",
                                                    "Kill a single enemy",
                                                    Resources.Load<Sprite>("Icons/" + 12)); // STEAM IMPLEMENTED w/ tracker

        achievementInformation[13] = new Achievement(achievements[13],
                                                    "Certified Danger",
                                                    "Kill 100 enemies",
                                                    Resources.Load<Sprite>("Icons/" + 13)); // STEAM IMPLEMENTED w/ tracker

        achievementInformation[14] = new Achievement(achievements[14],
                                                    "Classified lethal weapon",
                                                    "Kill 1000 enemies",
                                                    Resources.Load<Sprite>("Icons/" + 14)); // STEAM IMPLEMENTED w/ tracker

        achievementInformation[15] = new Achievement(achievements[15],
                                                    "This is getting crazy",
                                                    "Kill 10,000 enemies",
                                                    Resources.Load<Sprite>("Icons/" + 15)); // STEAM IMPLEMENTED w/ tracker

        achievementInformation[16] = new Achievement(achievements[16],
                                                   "Untouchable",
                                                   "Beat a room without taking damage",
                                                   Resources.Load<Sprite>("Icons/" + 16)); // STEAM IMPLEMENTED

        achievementInformation[17] = new Achievement(achievements[17],
                                                   "Part-time ghost",
                                                   "Beat a level without taking damage",
                                                   Resources.Load<Sprite>("Icons/" + 17)); // STEAM IMPLEMENTED

        achievementInformation[18] = new Achievement(achievements[18],
                                                   "The Matrix",
                                                   "Dodge roll through a bullet while adrenaline pills are active",
                                                   Resources.Load<Sprite>("Icons/" + 18));  // STEAM IMPLEMENTED

        achievementInformation[19] = new Achievement(achievements[19],
                                                   "Cheaters always win",
                                                   "Use the cheat code to access the secret Encylcopedia menu",
                                                   Resources.Load<Sprite>("Icons/" + 19));  // STEAM IMPLEMENTED

        achievementInformation[20] = new Achievement(achievements[20],
                                                   "Hippocratic oath",
                                                   "Defeat the one who heals",
                                                   Resources.Load<Sprite>("Icons/" + 20));  // STEAM IMPLEMENTED

        achievementInformation[21] = new Achievement(achievements[21],
                                                   "Shiny Hunter",
                                                   "Find and kill a shiny minion",
                                                   Resources.Load<Sprite>("Icons/" + 21));  // STEAM IMPLEMENTED

        achievementInformation[22] = new Achievement(achievements[22],
                                                   "Look, I got the whole set",
                                                   "Kill every type of cultist",
                                                   Resources.Load<Sprite>("Icons/" + 22));  // STEAM IMPLEMENTED

        achievementInformation[23] = new Achievement(achievements[23],
                                                   "Fantastical Warfare",
                                                   "Kill every enemy not wielding a gun",
                                                   Resources.Load<Sprite>("Icons/" + 23)); // STEAM IMPLEMENTED

        achievementInformation[24] = new Achievement(achievements[24],
                                                   "Man at Arms",
                                                   "Pick up every gun",
                                                   Resources.Load<Sprite>("Icons/" + 24));  // STEAM IMPLEMENTED, no tracker
        achievementInformation[25] = new Achievement(achievements[25],
                                                   "Penny Pincher",
                                                   "Collect 10 Byle coins",
                                                   Resources.Load<Sprite>("Icons/" + 25));  // STEAM IMPLEMENTED w/ tracker
        achievementInformation[26] = new Achievement(achievements[26],
                                                   "Stonks",
                                                   "Collect 1,000 Byle coins",
                                                   Resources.Load<Sprite>("Icons/" + 26));  // STEAM IMPLEMENTED w/ tracker
        achievementInformation[27] = new Achievement(achievements[27],
                                                   "Byllionaire",
                                                   "Collect 10,000 Byle Coins",
                                                   Resources.Load<Sprite>("Icons/" + 27));  // STEAM IMPLEMENTED w/ tracker

        achievementInformation[28] = new Achievement(achievements[28],
                                                   "Hell on earth",
                                                   "Kill the Revenenant",
                                                   Resources.Load<Sprite>("Icons/" + 28)); // STEAM IMPLEMENTED

        achievementInformation[29] = new Achievement(achievements[29],
                                                   "Mike Main",
                                                   "Get through all level themes with Mike",
                                                   Resources.Load<Sprite>("Icons/" + 29)); // STEAM IMPLEMENTED

        achievementInformation[30] = new Achievement(achievements[30],
                                                   "Rambo Realist",
                                                   "Get through all level themes with Rambo",
                                                   Resources.Load<Sprite>("Icons/" + 30)); // STEAM IMPLEMENTED

        achievementInformation[31] = new Achievement(achievements[31],
                                                   "Vlad Visionary",
                                                   "Get through all level themes with Vlad",
                                                   Resources.Load<Sprite>("Icons/" + 31)); // STEAM IMPLEMENTED

        achievementInformation[32] = new Achievement(achievements[32],
                                                   "One Eye Optimist",
                                                   "Get through all level themes with Ol One Eye",
                                                   Resources.Load<Sprite>("Icons/" + 32)); // STEAM IMPLEMENTED

        achievementInformation[33] = new Achievement(achievements[33],
                                                   "Dart Director",
                                                   "Get through all level themes with Dart",
                                                   Resources.Load<Sprite>("Icons/" + 33)); // STEAM IMPLEMENTED

        achievementInformation[34] = new Achievement(achievements[34],
                                                   "Pure Commitment",
                                                   "Get through 20 levels in a row",
                                                   Resources.Load<Sprite>("Icons/" + 34)); // STEAM IMPLEMENTED

        achievementInformation[35] = new Achievement(achievements[35],
                                                   "Narrow Escape",
                                                   "Complete a level with less than 10% of your healh remaining",
                                                   Resources.Load<Sprite>("Icons/" + 35));   // STEAM IMPLEMENTED

        achievementInformation[36] = new Achievement(achievements[36],
                                                   "Get Luke'd",
                                                   "Find Luke",
                                                   Resources.Load<Sprite>("Icons/" + 36)); // STEAM IMPLEMENTED, there is no actual achievement on steam however

        achievementInformation[37] = new Achievement(achievements[37],
                                                   "BYLE Super Fan",
                                                   "Unlock all achievements",
                                                   Resources.Load<Sprite>("Icons/" + 37)); // STEAM IMPLEMENTED
        achievementInformation[38] = new Achievement(achievements[38],
                                                   "Boris Begone",
                                                   "Defeat Boris",
                                                   Resources.Load<Sprite>("Icons/" + 38)); // STEAM IMPLEMENTED

        achievementInformation[39] = new Achievement(achievements[39],
                                                   "Pistol Connoisseur",
                                                   "Beat level 1 only holding a pistol",
                                                   Resources.Load<Sprite>("Icons/" + 39)); // STEAM IMPLEMENTED

        achievementInformation[40] = new Achievement(achievements[40],
                                                   "End The Cult",
                                                   "Kill the mega Cultist",
                                                   Resources.Load<Sprite>("Icons/" + 40)); // STEAM IMPLEMENTED

        achievementInformation[41] = new Achievement(achievements[41],
                                                      "Jugger-not",
                                                      "Kill the Juggernaut",
                                                      Resources.Load<Sprite>("Icons/" + 41)); // STEAM IMPLEMENTED

        achievementInformation[42] = new Achievement(achievements[42],
                                                      "Fireman",
                                                      "Kill the Incinerator",
                                                      Resources.Load<Sprite>("Icons/" + 42)); // STEAM IMPLEMENTED

        achievementInformation[43] = new Achievement(achievements[43],
                                                      "Armageddon",
                                                      "Kill the Armada",
                                                      Resources.Load<Sprite>("Icons/" + 43)); // STEAM IMPLEMENTED

        achievementInformation[44] = new Achievement(achievements[44],
                                                     "Poor Mans Magic",
                                                     "Kill the Necromancer",
                                                     Resources.Load<Sprite>("Icons/" + 44)); // STEAM IMPLEMENTED

        achievementInformation[45] = new Achievement(achievements[45],
                                                     "The Whopper",
                                                     "Kill the Triplestack",
                                                     Resources.Load<Sprite>("Icons/" + 45)); // STEAM IMPLEMENTED

        achievementInformation[46] = new Achievement(achievements[46],
                                                     "Pile Up",
                                                     "Kill the Abomination",
                                                     Resources.Load<Sprite>("Icons/" + 46)); // STEAM IMPLEMENTED

        achievementInformation[47] = new Achievement(achievements[47],
                                                     "Nice",
                                                     "Reach wave 69 in the Colosseum",
                                                     Resources.Load<Sprite>("Icons/" + 47)); // STEAM IMPLEMENTED

        achievementInformation[48] = new Achievement(achievements[48],
                                                   "Karlos Conquest",
                                                   "Get through all level themes with Karlos",
                                                   Resources.Load<Sprite>("Icons/" + 48)); // STEAM IMPLEMENTED

        achievementInformation[49] = new Achievement(achievements[49],
                                                   "Byleaholic",
                                                   "Drink the BYLE",
                                                   Resources.Load<Sprite>("Icons/" + 49)); // IMPLEMETED

        achievementInformation[50] = new Achievement(achievements[50],
                                                   "Xerxes Extremist",
                                                   "Get through all level themes with Xerxes",
                                                   Resources.Load<Sprite>("Icons/" + 50)); // IMPLEMETED

        achievementInformation[51] = new Achievement(achievements[51],
                                                  "Surfing Spirit",
                                                  "Beat 10 waves without taking damage",
                                                  Resources.Load<Sprite>("Icons/" + 51)); // IMPLEMETED

        achievementInformation[52] = new Achievement(achievements[52],
                                                  "Wave Phantom",
                                                  "Beat 20 waves without taking damage",
                                                  Resources.Load<Sprite>("Icons/" + 52)); // IMPLEMETED

        achievementInformation[53] = new Achievement(achievements[53],
                                                  "Tsunami Ghost",
                                                  "Beat 30 waves without taking damage",
                                                  Resources.Load<Sprite>("Icons/" + 53)); // IMPLEMETED

        achievementInformation[54] = new Achievement(achievements[54],
                                                  "Swordfight",
                                                  "Kill Ruku with a katana",
                                                  Resources.Load<Sprite>("Icons/" + 54)); // IMPLEMETED

        achievementInformation[55] = new Achievement(achievements[55],
                                                  "Speedy Gonzales",
                                                  "Have the equivalent of 5 speed boosts",
                                                  Resources.Load<Sprite>("Icons/" + 55)); // IMPLEMETED

        achievementInformation[56] = new Achievement(achievements[56],
                                                  "Power House",
                                                  "Have the equivalent of 5 attack boosts",
                                                  Resources.Load<Sprite>("Icons/" + 56)); // IMPLEMETED

        achievementInformation[57] = new Achievement(achievements[57],
                                                  "Brick Wall",
                                                  "Have over 210 health",
                                                  Resources.Load<Sprite>("Icons/" + 57)); // IMPLEMETED

        achievementInformation[58] = new Achievement(achievements[58],
                                                  "Die Hard",
                                                  "Beat 30 waves without picking up a weapon",
                                                  Resources.Load<Sprite>("Icons/" + 58)); 
    }
    
    public static void UnlockAchievement(int index)
    {
        // If achievement is already obtained then exit the function
        if (achievements[index]) 
        { 
            return;
        }

        // Sets the achievements flag to true
        achievements[index] = true;
        SaveLoadSystem.instance.SaveAchievements();
        Init();

        // Gets the title and description of the achievement
        string title = "", description = "";
        Sprite icon = null;

        Debug.Log(achievementInformation[index].Title);
        title = achievementInformation[index].Title;
        description = achievementInformation[index].Description;
        icon = achievementInformation[index].Icon;

        // Sends the achievement pop up
        AchievementPopup.instance.SetUIText(title, description, icon);
        AchievementPopup.instance.StartPopUp();
    }
}

[System.Serializable]
public struct Achievement
{
    public bool Obtained;
    public string Title;
    public string Description;
    public Sprite Icon;
  
    public Achievement(bool obtained, string title, string description, Sprite icon)
    {
        Obtained = obtained;
        Title = title;
        Description = description;
        Icon = icon;
    }
}
