using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    string username;                                   // What is the name of the player
    int score;                                       // How much score does the player currently have
    float moneySpent;                                  // How much money has the player spent
    float money;                                       // How much money does the player currently have (based off of moneySpent and score)
    enum ranks {POOP, BRONZE, SILVER, GOLD};           // Enum of the ranks so the code is more organized
    string rank;                                       // How well is the player doing

    bool saved;                                        // Bool to tell if the player's score has been saved

    private TMP_Text scoreText;                        // Displays the amount of score the player has
    private TMP_Text rankText;                         // Displayes the rank that the player has
    private TMP_Text moneyText;                        // Displays the amount of money the player has

    private float sTime;                                // Time since start()
    private float eTime;                                // Time elapsed since sTime
    private LaneSwitcher playerInfo;                    // Reference to the WheelVehicle script
    private float[] speeds;                             // Holds 30 speeds, used to calculate aSpeed
    private float cTime;                                // Holds the last time the player's speed was checked
    private float aSpeed;                               // Averages speed of the player over the last 30 seconds
    public float storeScoreMult = 1f;                   // The value of the Store-bought Score Multiplier upgrade

    // Start is called before the first frame update
    void Start()
    {
        username = "Player9";
        score = 0;
        moneySpent = 0;
        money = score - moneySpent;

        saved = false;

        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<TMP_Text>();
        rankText = GameObject.FindGameObjectWithTag("Rank").GetComponent<TMP_Text>();
        moneyText = GameObject.FindGameObjectWithTag("Money").GetComponent<TMP_Text>();
        redisplayInfo();

        sTime = Time.time;
        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<LaneSwitcher>();
        speeds = new float[30];
        speeds[0] = playerInfo.Speed;
        cTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!saved && !playerInfo.gameObject.GetComponent<PlayerHealth>().dead)
        {
            eTime = Time.time - sTime;

            calcAvgSpeed();

            calcScore();

            calcRank();

            redisplayInfo();
        }
    }





    // Polls the speed and Calculates the average speed of the player over the last speeds.Length seconds
    void calcAvgSpeed()
    {
        // add speed to averages every second or so
        if (Time.time - cTime >= 1f)
        {
            // only get last 30 speeds (% 30)
            speeds[(ulong)Mathf.Floor(eTime) % 30] = playerInfo.Speed;
            cTime = Time.time;
        }

        for (int i = 0; i < speeds.Length; ++i)
        {
            aSpeed += speeds[i];
        }

        aSpeed /= speeds.Length;
        //print("aSpeed" + aSpeed);
    }

    // Returns a speed multiplier determined by an average speed
    float getSpeedMult(float _aSpeed)
    {
        if (_aSpeed > 100f && _aSpeed <= 1000f)
        {
            return (float)(1 + _aSpeed / 10f) * storeScoreMult;
            
            // Is this what we wanted for this upgrade^^^?
        }
        else if (_aSpeed > 1000f)
        {
            
            return (float)(1 + _aSpeed / 100f) * storeScoreMult;
        }

        // base case - aSpeed < 100f
        return (float)(1 + _aSpeed / 10f) * storeScoreMult;
    }

    // Calculates the score, rank, money (since money and rank are dervied from score), and the related information
    void calcScore()
    {
        // calculates the score, getSpeedMult(aSpeed) is only called if 30 seconds have passed
        int tempScore = (int)Mathf.Floor(1.5f * Mathf.Pow(eTime, 2f) * getSpeedMult(aSpeed));
        //print("Mathf.Pow(eTime, 2f) = " + Mathf.Pow(eTime, 2f) + "getSpeedMult( " + aSpeed + " ) =" + getSpeedMult(aSpeed));

        // only increase the score
        score = tempScore > score ? tempScore : score;
        //if (tempScore < score) print("tempScore" + tempScore + " score: " + score); ;
        //print("Score: " + score + " tempScore: " + tempScore);
        money = score / 4 - moneySpent;

    }

    // Calculate the rank based on the score of the player
    void calcRank()
    {
        if (score < 1000)
            rank = ranks.POOP.ToString();
        else if (score >= 1000 && score < 10000)
            rank = ranks.BRONZE.ToString();
        else if (score >= 10000 && score < 100000)
            rank = ranks.SILVER.ToString();
        else if (score >= 100000)
            rank = ranks.GOLD.ToString();
    }





    // Updates the score, rank, and money (since money and rank are derived from score) UI elements
    void redisplayInfo()
    {
        scoreText.text = "Score: " + score.ToString();
        moneyText.text = "$" + money.ToString();
        
        if(rank == ranks.POOP.ToString())
        {
            rankText.color = new Color(.7f, .35f, 0f);
        }
        else if(rank == ranks.BRONZE.ToString())
        {
            rankText.color = new Color(.7f, .6f, 0f);
        }
        else if(rank == ranks.SILVER.ToString())
        {
            rankText.color = Color.gray;
        }
        else
        {
            rankText.color = Color.yellow;
        }
        
        rankText.text = "Rank: " + rank;
    }



    // accessor for money
    public float getMoney()
    {
        return money;
    }

    // Adds amnt to moneySpent
    public bool spendMoney(float amnt)
    {
        if (amnt <= money)
            moneySpent += amnt;
        else
            return false;

        return true;
    }

    // Save the player's score and rank
    public void saveScore()
    {
        Debug.Log("Saving Score");

        // load the save and obtain the data
        SaveGame.Load();

        // retrieve the saved information
        int numSavedPlayers = SaveGame.Instance.numPlayers;
        string[] savedPlayers = SaveGame.Instance.players;
        int[] savedScores = SaveGame.Instance.scores;

        bool save = true; ;
        int toWrite = numSavedPlayers;

        // we need to overwrite an entry
        if (toWrite == 99)
        {
            // find the lowest score 
            toWrite = savedScores[0];
            for (int i = 1; i < savedScores.Length; ++i)
            {
                if (savedScores[i] < toWrite)
                    toWrite = i;
            }

            if (savedScores[toWrite] >= score)
                save = false;
        }

        if (save)
        {
            // check to see if the player already has an entry
            bool exists = false;

            // itereate through the players and check for username
            for (int i = 0; i < numSavedPlayers; ++i)
            {
                if (username == savedPlayers[i])
                {
                    exists = true;
                    toWrite = i;
                }
            }

            // create an entry or update an existing one
            if (score > savedScores[toWrite])
            {
                // create new entry
                if (!exists)
                {
                    SaveGame.Instance.players[toWrite] = username;
                    ++numSavedPlayers;
                    SaveGame.Instance.numPlayers = Mathf.Min(numSavedPlayers, 100);
                }

                // store the score and rank for the entry being handled
                SaveGame.Instance.scores[toWrite] = score;
                SaveGame.Instance.ranks[toWrite] = rank;
            }

            // save the data
            SaveGame.Save();
        }

        // set the saved flag to true
        saved = true;

        // disable player input
        playerInfo.DisableMovement = true;
    }
}


