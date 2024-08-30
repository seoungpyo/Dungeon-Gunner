using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class HighScoreManager : SingletonMonobehaviour<HighScoreManager>
{
    private HighScores highScores = new HighScores();

    protected override void Awake()
    {
        base.Awake();

        LoadScores();
    }

    /// <summary>
    /// Load Scores from disk
    /// </summary>
    private void LoadScores()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/DungeonGunnerHighScores.dat"))
        {
            ClearScoreList();

            FileStream file = File.OpenRead(Application.persistentDataPath + "/DungeonGunnerHighScores.dat");

            highScores = (HighScores)bf.Deserialize(file);

            file.Close();

        }
    }

    /// <summary>
    /// Clear all scores
    /// </summary>
    private void ClearScoreList()
    {
        highScores.scoreList.Clear();
    }

    /// <summary>
    /// AAdd score to high scores list
    /// </summary>
    /// <param name="score"></param>
    /// <param name="rank"></param>
    public void AddScore(Score score, int rank)
    {
        highScores.scoreList.Insert(rank - 1, score);

        if(highScores.scoreList.Count > Settings.numberOfHighScoresToSave)
        {
            highScores.scoreList.RemoveAt(Settings.numberOfHighScoresToSave);
        }

        SaveScores();
    }

    /// <summary>
    /// Save Scores To Disk
    /// </summary>
    private void SaveScores()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/DungeonGunnerHighScores.dat");

        bf.Serialize(file, highScores);

        file.Close();
    }

    public HighScores GetHighScores()
    {
        return highScores;
    }

    /// <summary>
    /// Return the rank of the playerScore compared to the other high scores (returns 0 if the score isn't higher than any in the high scores list)
    /// </summary>
    /// <param name="playerScore"></param>
    /// <returns></returns>
    public int GetRank(long playerScore)
    {
        if (highScores.scoreList.Count == 0) return 1;

        int index = 0;

        for (int i = 0; i < highScores.scoreList.Count; i++)
        {
            index++;

            if (playerScore >= highScores.scoreList[i].playerScore)
            {
                return index;
            }   
        }

        if (highScores.scoreList.Count < Settings.numberOfHighScoresToSave) return (index + 1);

        return 0;
    }
}
