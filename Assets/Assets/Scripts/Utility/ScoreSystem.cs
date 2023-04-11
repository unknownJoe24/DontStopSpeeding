using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    string username;                                   // What is the name of the player
    ulong score;                                       // How much score does the player currently have
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
    private VehicleBehaviour.WheelVehicle playerInfo;   // Reference to the WheelVehicle script
    private float[] speeds;                             // Holds 30 speeds, used to calculate aSpeed
    private float cTime;                                // Holds the last time the player's speed was checked
    private float aSpeed;                               // Averages speed of the player over the last 30 seconds

    // Start is called before the first frame update
    void Start()
    {
        username = "Player1";
        score = 0;
        moneySpent = 0;
        money = score - moneySpent;

        saved = false;

        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<TMP_Text>();
        rankText = GameObject.FindGameObjectWithTag("Rank").GetComponent<TMP_Text>();
        moneyText = GameObject.FindGameObjectWithTag("Money").GetComponent<TMP_Text>();
        redisplayInfo();

        sTime = Time.time;
        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<VehicleBehaviour.WheelVehicle>();
        speeds = new float[30];
        speeds[0] = playerInfo.Speed;
        cTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!saved)
        {
            eTime = Time.time - sTime;

            calcAvgSpeed();

            calcScore();

            calcRank();

            if (Input.GetButtonDown("Defuse Bomb"))
                saveScore();

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
    }

    // Returns a speed multiplier determined by an average speed
    float getSpeedMult(float _aSpeed)
    {
        if (_aSpeed > 100f && _aSpeed <= 1000f)
        {
            return (float)(1 + _aSpeed / 100f);
        }
        else if (_aSpeed > 1000f)
        {
            return (float)(1 + _aSpeed / 1000f);
        }
        

        // base case - aSpeed < 100f
        return (float)(1 + _aSpeed / 10f);
    }

    // Calculates the score, rank, money (since money and rank are dervied from score), and the related information
    void calcScore()
    {
        // calculates the score, getSpeedMult(aSpeed) is only called if 30 seconds have passed
        ulong tempScore = (ulong)Mathf.Floor(1.5f * Mathf.Pow(eTime, 2f) * getSpeedMult(aSpeed));

        // only increase the score
        score = tempScore > score ? tempScore : score;

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
        scoreText.text = "Score\n" + score.ToString();
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
        
        rankText.text = "Rank\n" + rank;
        
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
        if((ulong)PlayerPrefs.GetInt(username + "Score") < score)
            PlayerPrefsHandler.saveScore(username, score, rank);
        saved = true;

        playerInfo.disableInput();

        Debug.Log(username + "'s Score: " + score + "\n" + username + "'s Rank: " + rank);
    }
}


