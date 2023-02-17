using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is a 'singleton', not how we have learned in the past, but it should serve our purposes here
// While this is a fine temporary system, it may need to be replaced later as each user requires two PlayerPrefss
static public class PlayerPrefsHandler
{
    // modifiers
    static public void saveScore(string _playerName, ulong _score, string _rank)
    {
        PlayerPrefs.SetInt(_playerName + "Score", (int)_score);
        PlayerPrefs.SetString(_playerName + "Rank", _rank);
    }

    // accessors
    static public int getScore(string _playerName)
    {
        return PlayerPrefs.GetInt(_playerName + "Score");
    }

    static public string getRank(string _playerName)
    {
        return PlayerPrefs.GetString(_playerName + "Rank");
    }
}


// Global References:
// Various videos and websites
// Other course materials such as CS3 slides
// https://stackoverflow.com/questions/31254387/how-do-i-use-an-array-in-a-switch-statement-in-c