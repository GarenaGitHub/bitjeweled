using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ScoresManager : ScriptableObject
{
    public static GUIText scoreOnScreen;

    private static ScoresManager instance = null;
    private static string workingFolder = "";
    private static string fullFileName = "";
    private static int currentScore = 0;
    private static ScoreTable scores = null;
    private static string currentUser = "";
    private static gameState userState = null;

    public static ScoresManager Instance
    {
        get
        {
            return instance;
        }
    }

    void OnEnable()
    {
        InitMe();
    }

    private void InitMe()
    {
        instance = this;
        workingFolder = Application.persistentDataPath;
        fullFileName = Path.Combine(workingFolder, "savegame.dat");
        if (File.Exists(fullFileName))
            LoadData();
        else
        {
            scores = new ScoreTable();
        }
    }


    public static string CurrentUser
    {
        get
        {
            return currentUser;
        }
        set
        {
            currentUser = value;
            if (scores.ContainsKey(currentUser))
                userState = scores[currentUser];
            else
            {
                userState = new gameState();
                userState.PlayerName = currentUser;
                userState.CurrentLevel = 1;
                userState.CurrentStage = 1;
            }
        }
    }

    public static int CurrentLevel
    {
        get
        {
            if (userState == null)
            {
                return 1;
            }
            return userState.CurrentLevel;
        }
        set
        {
            if (userState != null)
                userState.CurrentLevel = value;
        }
    }

    public static int CurrentStage
    {
        get
        {
            if (userState == null)
            {
                return 1;
            }
            return userState.CurrentStage;
        }
        set
        {
            userState.CurrentStage = value;
        }
    }

    public static void AddPoints(int pointsToAdd)
    {
        currentScore += pointsToAdd;
        if (userState != null)
            userState.TotalScore += pointsToAdd;
    }

    public static int CurrentPoints
    {
        get
        {
            return currentScore;
        }
    }

    public void LoadGameData()
    {
        Stream stream = File.Open(fullFileName, FileMode.Open);
        BinaryFormatter bFormatter = new BinaryFormatter();
        scores = bFormatter.Deserialize(stream) as ScoreTable;
        stream.Close();
        if (scores == null)
            scores = new ScoreTable();
    }

    public static void LoadData()
    {
        Instance.LoadGameData();
    }

    public void SaveGameData()
    {
        Stream stream = File.Open(fullFileName, FileMode.Create);
        BinaryFormatter bFormatter = new BinaryFormatter();
        bFormatter.Serialize(stream, scores);
        stream.Close();
    }

    public static void SaveData()
    {
        Instance.SaveGameData();
    }
}
