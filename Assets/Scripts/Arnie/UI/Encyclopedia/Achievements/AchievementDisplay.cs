using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDisplay : MonoBehaviour
{
    public Text Title;
    public Text Description;
    public Image Icon;

    public Image Background;
    public Image LockedSprite;

    public int achievementIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        AchievementSystem.Init();

        // Deletes the display if this achievment hasnt been implmented yet
        if(AchievementSystem.achievementInformation[achievementIndex].Title == null)
        {
            Destroy(this.gameObject);
        }

        // Sets all the appropriate UI elements to represent the achivement
        Title.text = AchievementSystem.achievementInformation[achievementIndex].Title;
        Description.text = AchievementSystem.achievementInformation[achievementIndex].Description;
        Icon.sprite = AchievementSystem.achievementInformation[achievementIndex].Icon;

        if (AchievementSystem.achievementInformation[achievementIndex].Obtained == true)
        {
            Background.color = new Color(0.4f, 0.8f, 0.5f, 1.0f);
            LockedSprite.gameObject.SetActive(false);
            Title.fontSize = 64;
            Icon.color = Color.white;
        }
        else
        {
            Background.color = Color.grey;
            LockedSprite.gameObject.SetActive(true);
            Title.fontSize = 52;
            Icon.color = Color.gray;
        }
    }

    private void Awake()
    {
        AchievementSystem.Init();
    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    AchievementSystem.UnlockAchievement(achievementIndex);
        //    AchievementSystem.Init();
        //    SaveLoadSystem.instance.SaveAchievements();
        //    this.Start();
        //}
        
    }
}
