using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class AchievementData 
{
    public int enemiesKilled;
    public int coinsCollected;
    public int roomsEntered;
    public float totalPlayTime;
    public bool[] weaponsPickedUp = new bool[30];
    public bool[] enemiesEncountered = new bool[69];
    public bool[] powerupsPickedUp = new bool[30];
    public AchievementData(int enemiesKilled, int coinsCollected, int roomsEntered, float totalPlayTime, bool[] weaponsPickedUp, bool[] enemiesEncountered, bool[] powerupsPickedUp)
    {
        this.enemiesKilled = enemiesKilled;
        this.coinsCollected = coinsCollected;
        this.roomsEntered = roomsEntered;
        this.totalPlayTime = totalPlayTime;
        this.weaponsPickedUp = weaponsPickedUp;
        this.enemiesEncountered = enemiesEncountered;
        this.powerupsPickedUp = powerupsPickedUp;
    }
}