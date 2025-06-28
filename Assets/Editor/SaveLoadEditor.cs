using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class SaveLoadEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameManager gameManager = (GameManager)target;

        if (DrawDefaultInspector())
        {
            if (gameManager.saveGame)
            {
                gameManager.SaveGameData();
            }
            if (gameManager.loadGame)
            {
                gameManager.LoadPlayerData();
                gameManager.LoadMapData();
            }
        }

        if (GUILayout.Button("Save Game"))
        {
            gameManager.SaveGameData();
        }
        if (GUILayout.Button("Load Game"))
        {
            gameManager.LoadPlayerData();
            gameManager.LoadMapData();
        }
        if (GUILayout.Button("Delete Save"))
        {
            SaveLoadSystem.instance.ResetData();
        }
        if(GUILayout.Button("Delete Achievements"))
        {
            SaveLoadSystem.instance.ResetAchievements();
        }
        if (GUILayout.Button("Generate"))
        {
            GameController.instance.GenerateMap();
        }

        if (GUILayout.Button("Generate With Seed"))
        {
            GameController.instance.GenerateMapWithSeed();
        }
        if (GUILayout.Button("Load next level"))
        {
            gameManager.OnWin();
        }
    }
}
