using UnityEngine;

public class SaveGame
{

    //serialized
    public int numPlayers = 0;
    public string[] players = new string[100];
    public int[] scores = new int[100];
    public string[] ranks = new string[100];

    private static string _gameDataFileName = "HighScores(1).json";

    private static SaveGame _instance;
    public static SaveGame Instance
    {
        get
        {
            if (_instance == null)
                Load();
            return _instance;
        }

    }

    public static void Save()
    {
        FileManager.Save(_gameDataFileName, _instance);
    }

    public static void Load()
    {
        _instance = FileManager.Load<SaveGame>(_gameDataFileName);
    }
}

// saving help
//https://www.youtube.com/watch?v=51y8kU_nEvc
//https://gist.github.com/ditzel/614f2ddc9cda11488602a27a6ced70a5
//https://www.youtube.com/watch?v=6uMFEM-napE
//https://www.youtube.com/watch?v=aUi9aijvpgs

// Global References:
// Various videos and websites
// Other course materials such as CS3 slides
// https://stackoverflow.com/questions/31254387/how-do-i-use-an-array-in-a-switch-statement-in-c
// https://www.youtube.com/watch?v=lq0RGtwmvmc - scrolling
// https://gamedevbeginner.com/the-right-way-to-pause-the-game-in-unity/#:~:text=The%20most%20convenient%20method%20for,game%20to%20its%20normal%20speed - time