using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SaveData
{
    public string name;
    public int score;
    public float maxHealth;
    public float health;

    public string weapon1;
    public string weapon2;

    public List<int> modifierType;
    public List<float> modAmount;

    public PlayerController.SelectedCharacter savedCharacter; 
    public string activeAbility;

    public int currentLevel;
    public int levelSeed;

    public SaveData(string name, int score, //float maxHealth, 
        float health, string weapon1, string weapon2, List<int> modifierType, List<float> modAmount, PlayerController.SelectedCharacter savedCharacter, string activeAbility,
        int currentLevel, int levelSeed)
    {
        this.name = name;
        this.score = score;
        //this.maxHealth = maxHealth;
        this.health = health;
        this.weapon1 = weapon1;
        this.weapon2 = weapon2;

        this.modifierType = modifierType;
        this.modAmount = modAmount;

        this.savedCharacter = savedCharacter;
        this.activeAbility = activeAbility;

        this.currentLevel = currentLevel;
        this.levelSeed = levelSeed;
    }

    
}
