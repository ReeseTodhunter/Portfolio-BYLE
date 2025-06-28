using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameController gameController = (GameController)target;

        if (DrawDefaultInspector())
        {
            if (gameController.generate)
            {
                gameController.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            gameController.GenerateMap();
        }

        if (GUILayout.Button("Generate With Seed"))
        {
            gameController.GenerateMapWithSeed();
        }

    }
}
