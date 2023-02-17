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
    string rank;                                       // How well is the player doing

    bool saved;                                        // Bool to tell if the player's score has been saved

    private TMP_Text scoreText;                        // Displays the amount of score the player has
    private TMP_Text rankText;                         // Displayes the rank that the player has
    private TMP_Text moneyText;                        // Displays the amount of money the player has

    private float sTime;                                // Time since start()
    private float eTime;                                // Time elapsed since sTime
    private Rigidbody playerRigidBody;                  // The rigid body of the player
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
        playerRigidBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
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
    int getSpeedMult(float _aSpeed)
    {
        // prevent the player from getting a speed multiplier before the full 30 seconds -- can be removed
        if (eTime <= 30f)
            return 1;

        if (_aSpeed > 60f && _aSpeed <= 80f)
        {
            return 2;
        }
        else if (_aSpeed > 80f && _aSpeed <= 100f)
        {
            return 3;
        }
        else if (_aSpeed > 100f)
        {
            return 4;
        }

        // base case - aSpeed <= 60f
        return 1;
    }

    // Calculates the score, rank, money (since money and rank are dervied from score), and the related information
    void calcScore()
    {
        // calculates the score, getSpeedMult(aSpeed) is only called if 30 seconds have passed
        ulong tempScore = (ulong)Mathf.Floor(1.5f * Mathf.Pow(eTime, 2f) * (eTime >= 30f ? getSpeedMult(aSpeed) : 1f));
        // only increase the score
        score = tempScore > score ? tempScore : score;

        money = score / 4 - moneySpent;
    }

    // Calculate the rank based on the score of the player
    void calcRank()
    {
        if (score < 1000)
            rank = "POOP";
        else if (score >= 1000 && score < 10000)
            rank = "BRONZE";
        else if (score >= 10000 && score < 100000)
            rank = "SILVER";
        else if (score >= 100000)
            rank = "GOLD";
    }





    // Updates the score, rank, and money (since money and rank are derived from score) UI elements
    void redisplayInfo()
    {
        scoreText.text = "Score\n" + score.ToString();
        moneyText.text = "$" + money.ToString();

        switch(rank)
        {
            case "POOP":
                rankText.color = new Color(.7f, .35f, 0f);
                break;
            case "BRONZE":
                rankText.color = new Color(.7f, .6f, 0f);
                break;
            case "SILVER":
                rankText.color = Color.gray;
                break;
            case "GOLD":
                rankText.color = Color.yellow;
                break;
            default:
                break;
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
        PlayerPrefsHandler.saveScore(username, score, rank);
        saved = true;

        // this works, but I think the car itself has issues with staying completely still
        playerInfo.IsPlayer = false;
        playerInfo.Throttle = 0f;
        playerRigidBody.velocity = new Vector3(0f, 0f, 0f);

        Debug.Log(username + "'s Score: " + score + "\n" + username + "'s Rank: " + rank);
    }
}
