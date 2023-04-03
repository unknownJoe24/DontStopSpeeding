using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is a 'singleton', not how we have learned in the past, but it should serve our purposes here
// While this is a fine temporary system, it may need to be replaced later as each user requires two PlayerPrefss
static public class PlayerPrefsHandler
{
    static private string[] players = new string[100];  // stores the player names
    static int numPlayers = 0;                          // how many players are there

    // modifiers
    static public void saveScore(string _playerName, ulong _score, string _rank)
    {
        players[numPlayers++] = _playerName;

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

    static public string[] getPlayers()
    {
        return players;
    }

    static public int getNumPlayers()
    {
        return numPlayers;
    }

    static public string getPlayer(int _index)
    {
        return players[_index];
    }

    static public void testFunc()
    {
        players[0] = "Jeff";
        players[1] = "MustacheCashtache";
        players[2] = "Meldin";

        numPlayers = 3;

        PlayerPrefs.SetInt(players[0] + "Score", 1450);
        PlayerPrefs.SetInt(players[1] + "Score", 999999);
        PlayerPrefs.SetInt(players[2] + "Score", 2);

        PlayerPrefs.SetString(players[0] + "Rank", "BRONZE");
        PlayerPrefs.SetString(players[1] + "Rank", "DIAMOND");
        PlayerPrefs.SetString(players[2] + "Rank", "LITERALLY HOW");
    }
}


// Global References:
// Unity documentation
// Various videos and websites
// Other course materials such as CS3 slides
// https://stackoverflow.com/questions/31254387/how-do-i-use-an-array-in-a-switch-statement-in-c