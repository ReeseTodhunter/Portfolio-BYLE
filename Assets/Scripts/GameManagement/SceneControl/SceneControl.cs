using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    //Highscore Stuff will go here
    [SerializeField]
    HighScoreDisplay HighScoreDisplay;

    float counter = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameOver"))
        {
            //Display Score,
            //GameManager.GMinstance.GameScore;
            //Ask to type name in
            //Pass player score into highscore table if it's high enough
        }
        //counter -= Time.unscaledDeltaTime;
        //if(counter < 0)
        //{
        //    SceneManager.LoadScene("Leaderboard");
        //}

    }

    public void OnWin()
    {
        SceneManager.LoadScene("TemporaryWin");
    }

    public void OnLose()
    {
        //HighScoreDisplay.AddNewScore("Name", PlayerController.instance.score);
        //HighScoreDisplay.Save();
        //SceneManager.LoadScene("GameOver");
        //Pass player score into highscore table if it's high enough
        

    }
}
