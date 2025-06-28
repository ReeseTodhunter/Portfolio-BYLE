using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class KonamiCode : MonoBehaviour
{
    public Text keyStrokeText;
    public Text cheatCodeText;

    public GameObject DevPage;
    public bool devPageAdded = false;

    private List<string> _keyStrokeHistory;

    void Awake()
    {
        _keyStrokeHistory = new List<string>();
    }

    void Update()
    {
        KeyCode keyPressed = DetectKeyPressed();
        AddKeyStrokeToHistory(keyPressed.ToString());
        //keyStrokeText.text = "HISTORY: " + GetKeyStrokeHistory();
        if (GetKeyStrokeHistory().Equals("UpArrow,UpArrow,DownArrow,DownArrow,LeftArrow,RightArrow,LeftArrow,RightArrow,B,A"))
        {
            //cheatCodeText.text = "KONAMI CHEAT CODE DETECTED!";
            ClearKeyStrokeHistory();
            if(devPageAdded != true)
            {

                EncyclopediaScript.instance.Pages.Add(DevPage);
                EncyclopediaScript.instance.Start();
                devPageAdded = true;

                AchievementSystem.UnlockAchievement(19);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                GameManager.GMinstance.UnlockSteamAchievement("Cheaters_Always_Win");
            }
            
        }
    }

    private KeyCode DetectKeyPressed()
    {
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                return key;
            }
        }
        return KeyCode.None;
    }

    private void AddKeyStrokeToHistory(string keyStroke)
    {
        if (!keyStroke.Equals("None"))
        {
            _keyStrokeHistory.Add(keyStroke);
            if (_keyStrokeHistory.Count > 10)
            {
                _keyStrokeHistory.RemoveAt(0);
            }
        }
    }

    private string GetKeyStrokeHistory()
    {
        return String.Join(",", _keyStrokeHistory.ToArray());
    }

    private void ClearKeyStrokeHistory()
    {
        _keyStrokeHistory.Clear();
    }
}
