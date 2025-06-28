using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    List<HighScoreEntry> scores = new List<HighScoreEntry>();
    public HighScoreDisplay[] highScoreDisplayArray;
    public void Save()
    {
        XMLScore.instance.SaveScores(scores);
        Debug.Log("Saved");
    }

    void Load()
    {
        scores = XMLScore.instance.LoadScores();
    }


    public void DisplayHighScore(string name, int score)
    {
        nameText.text = name;
        scoreText.text = string.Format("{0:00000}", score);
    }
    public void HideEntryDisplay()
    {
        nameText.text = "";
        scoreText.text = "";

    }

    public void AddNewScore(string entryName, int entryScore)
    {
        scores.Add(new HighScoreEntry { name = entryName, score = entryScore });
    }
    // Start is called before the first frame update
    void Start()
    {
        /*AddNewScore("John", 4500);
        AddNewScore("Max", 5520);
        AddNewScore("Dave", 380);
        AddNewScore("Steve", 6654);
        AddNewScore("Mike", 11021);
        AddNewScore("Teddy", 3252);*/
        Load();
        UpdateDisplay();
    }

    // Update is called once per frame
    public void UpdateDisplay()
    {
        scores.Sort((HighScoreEntry x, HighScoreEntry y) => y.score.CompareTo(x.score));
        for (int i = 0; i < highScoreDisplayArray.Length; i++)
        {
            if (i < scores.Count)
            {
                highScoreDisplayArray[i].DisplayHighScore(scores[i].name, scores[i].score);
            }
            else
            {
                highScoreDisplayArray[i].HideEntryDisplay();
            }
        }

    }
}
public class HighScoreEntry
{
    public string name;
    public int score;
}
