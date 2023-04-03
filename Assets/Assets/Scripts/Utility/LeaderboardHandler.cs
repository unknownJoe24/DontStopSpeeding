using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardHandler : MonoBehaviour
{
    public Canvas LeaderboardCanvas;    // the canvas to place things under
    public GameObject entryTemplate;    // template for leaderboard entries

    private float initOffset;           // initial offset for the leaderboard entries
    private float center;               // the center of the canvas
    private float templateHeight;       // height of the templates

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefsHandler.testFunc();

        initOffset = 350f;
        center = 475f;
        templateHeight = 50f;

        string[] allPlayers = PlayerPrefsHandler.getPlayers();

        allPlayers = sortPlayers(allPlayers);

        for(int i = 0; i < PlayerPrefsHandler.getNumPlayers(); ++i)
        {
            // create a new entry
            RectTransform newTrans;
            GameObject curr = Instantiate(entryTemplate);
            curr.transform.SetParent(LeaderboardCanvas.transform);
            newTrans = curr.GetComponent<RectTransform>();
            newTrans.position = new Vector2(center, initOffset - templateHeight * i);

            
            // fill the entry
            string playerName = allPlayers[i];

            TMP_Text[] fields = curr.GetComponentsInChildren<TMP_Text>();
            
            for(int j = 0; j < fields.Length; ++j)
            {
                switch(fields[j].gameObject.name)
                {
                    case "Username":
                        fields[j].text = playerName;
                        break;

                    case "Score":
                        fields[j].text = PlayerPrefsHandler.getScore(playerName).ToString();
                        break;

                    case "Rank":
                        fields[j].text = PlayerPrefsHandler.getRank(playerName);
                        break;

                    default:
                        break;
                }
            }


        }
    }

    string[] sortPlayers(string[] _allPlayers)
    {
        // selection sort
        for(int i = 0; i < PlayerPrefsHandler.getNumPlayers(); ++i)
        {
            int highScore = -1;
            int highIndex = i;
            for (int j = i; j < PlayerPrefsHandler.getNumPlayers(); ++j)
            {
                int currScore = PlayerPrefsHandler.getScore(_allPlayers[j]);

                if (currScore > highScore)
                {
                    highScore = currScore;
                    highIndex = j;
                }
            }

            string temp = _allPlayers[highIndex];
            _allPlayers[highIndex] = _allPlayers[i];
            _allPlayers[i] = temp;
        }

        return _allPlayers;
    }
}
