using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardHandler_Beautified : MonoBehaviour
{
    public Canvas LeaderboardCanvas;    // the canvas to place things under
    public GameObject contentWindow;    // the object to place things under
    public GameObject entryTemplate;    // template for leaderboard entries

    private float initOffset;           // initial offset for the leaderboard entries
    private float centerx;              // the center of the canvas
    private float templateHeight;       // height of the templates

    // Start is called before the first frame update
    void Start()
    {
        // load the save data, and initialize data
        SaveGame.Load();

        centerx = LeaderboardCanvas.GetComponent<RectTransform>().rect.width / 2f;
        templateHeight = 50f;

        int numAllPlayers = SaveGame.Instance.numPlayers;
        string[] allPlayers = SaveGame.Instance.players;
        int[] allScores = SaveGame.Instance.scores;
        string[] allRanks = SaveGame.Instance.ranks;

        RectTransform contentWindowTrans = contentWindow.GetComponent<RectTransform>();
        initOffset = contentWindowTrans.rect.height / 2f - templateHeight;

        sortPlayers(ref allPlayers, ref allScores, ref allRanks);

        // create an entry for each set of player data
        for(int i = 0; i < numAllPlayers; ++i)
        {
            // create a new entry
            RectTransform newTrans;
            GameObject curr = Instantiate(entryTemplate);
            curr.transform.SetParent(contentWindow.transform);
            newTrans = curr.GetComponent<RectTransform>();
            newTrans.anchoredPosition = new Vector2(0, initOffset - templateHeight * i);

            
            // get the player information from the obtained save data
            string playerName = allPlayers[i];
            int playerScore = allScores[i];
            string playerRank = allRanks[i];

            // fill the information
            TMP_Text[] fields = curr.GetComponentsInChildren<TMP_Text>();
            
            for(int j = 0; j < fields.Length; ++j)
            {
                switch(fields[j].gameObject.name)
                {
                    case "Username":
                        fields[j].text = playerName;
                        break;

                    case "Score":
                        fields[j].text = playerScore.ToString();
                        break;

                    case "Rank":
                        fields[j].text = playerRank;
                        break;

                    default:
                        break;
                }
            }
        }
    }

    void sortPlayers(ref string[] _allPlayers, ref int[] _allScores, ref string[] _allRanks)
    {
        int totalNum = SaveGame.Instance.numPlayers;

        // selection sort
        for(int i = 0; i < totalNum; ++i)
        {
            int highScore = -1;
            int highIndex = i;
            for (int j = i; j < totalNum; ++j)
            {
                int currScore = _allScores[j];

                if (currScore > highScore)
                {
                    highScore = currScore;
                    highIndex = j;
                }
            }

            string pTemp = _allPlayers[highIndex];
            _allPlayers[highIndex] = _allPlayers[i];
            _allPlayers[i] = pTemp;

            int sTemp = _allScores[highIndex];
            _allScores[highIndex] = _allScores[i];
            _allScores[i] = sTemp;

            string rTemp = _allRanks[highIndex];
            _allRanks[highIndex] = _allRanks[i];
            _allRanks[i] = rTemp;
        }
    }
}
