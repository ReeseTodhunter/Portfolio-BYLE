using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballScript : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<GoalCollider>() != null)
        {
            //Future Star
            AchievementSystem.UnlockAchievement(11);
            AchievementSystem.Init();
            SaveLoadSystem.instance.SaveAchievements();
            GameManager.GMinstance.UnlockSteamAchievement("Future_Star");

            gameObject.GetComponent<AudioSource>().Play();
            CanvasScript.instance.roomClear.clearText.text = "GOLAZZOOOO";
            CanvasScript.instance.roomClear.roomClear = false;
            CanvasScript.instance.roomClear.Start();
        }
    }
}
