using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDisplayController : MonoBehaviour
{
    public static AchievementDisplayController instance;

    public int pageNumber = 1;
    public List<GameObject> Pages = new List<GameObject>();
    public Text pageText;


    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Check if all other achievements have been unlocked
        for (int i = 0; i < 57; ++i)
        {
            if (!AchievementSystem.achievements[i] && i != 36 && i != 37) return;
        }
        // Unlock all achivements achievement
        AchievementSystem.UnlockAchievement(37);
        AchievementSystem.Init();
        SaveLoadSystem.instance.SaveAchievements();
        GameManager.GMinstance.UnlockSteamAchievement("Byle_Super_Fan");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changePageNumber(int number)
    {
        if (pageNumber + number != 0 && pageNumber + number != Pages.Count + 1)
        {
            pageNumber = pageNumber + number;
        }

        foreach (GameObject page in Pages)
        {
            page.SetActive(false);
        }
        Pages[pageNumber - 1].SetActive(true);
        pageText.text = "PAGE " + pageNumber + "/" + Pages.Count;
    }
}
