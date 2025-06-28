using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    public Text scoreText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.GMinstance == null){return;}
        if (GameManager.GMinstance.GameScore > 0)
        {
            scoreText.text = "SCORE: " + GameManager.GMinstance.GameScore;
        }
        else
        {
            scoreText.text = "SCORE: " + PlayerController.instance.score;
        }
    }
}
