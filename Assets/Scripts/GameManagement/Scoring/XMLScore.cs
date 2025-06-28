using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class XMLScore : MonoBehaviour
{
    public static XMLScore instance;

    public Leaderboard leaderboard;

    private void Awake()
    {
        instance = this;
        
        if (!Directory.Exists(Application.persistentDataPath + "/HighScores/"))
        {
            Debug.Log("Save File Does Not Exist");
            Directory.CreateDirectory(Application.persistentDataPath + "/HighScores/");
        }
    }

    public void SaveScores(List<HighScoreEntry> scoresToSave)
    {
        leaderboard.scoreList = scoresToSave;
        XmlSerializer serializer = new XmlSerializer(typeof(Leaderboard));
        FileStream stream = new FileStream(Application.persistentDataPath + "/HighScores/highscores.xml", FileMode.Create);
        Debug.Log("Saved");
        serializer.Serialize(stream, leaderboard);
        stream.Close();
    }

    public List<HighScoreEntry> LoadScores()
    {
        if (File.Exists(Application.persistentDataPath + "/HighScores/highscores.xml"))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Leaderboard));
            FileStream stream = new FileStream(Application.persistentDataPath + "/HighScores/highscores.xml", FileMode.Open);
            leaderboard = serializer.Deserialize(stream) as Leaderboard;
        }
        return leaderboard.scoreList;
    }
}
[System.Serializable]
public class Leaderboard
{
    public List<HighScoreEntry> scoreList = new List<HighScoreEntry>();
}
