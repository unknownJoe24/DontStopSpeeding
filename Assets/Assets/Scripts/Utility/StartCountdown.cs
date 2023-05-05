using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartCountdown : MonoBehaviour
{
    public bool startCount = true;  // tells the script to count down or not
    public int firstCountLength = 5;     // how long does the countdown last the first time
    public int generalCountLength = 3;   // how long does the countdown last after the first time
    private bool firstCount;        // is this the countodwn for the beginning of the game
    private float timeSince;        // how long has it been since the countdown started

    private TextMeshProUGUI text;   // text itself
    
    // Start is called before the first frame update
    void Start()
    {
        firstCount = true;
        timeSince = 0;
        text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startCount)
            countdown();
    }

    private void countdown()
    {
        int countLength;
        if (firstCount)
            countLength = firstCountLength;
        else
            countLength = generalCountLength;

        // get the remaining time
        int remaining = countLength - Mathf.FloorToInt(timeSince);

        // "pause" the game
        if (remaining > 0)
            Time.timeScale = 0;

        // increment the timer
        timeSince += Time.unscaledDeltaTime;


        if (remaining > 0)
        {
            // set the text of the countdown
            changeColor(Mathf.FloorToInt(timeSince));
            text.text = remaining.ToString();
        }
        else if (remaining == 0)
        {
            changeColor(Mathf.FloorToInt(timeSince));
            text.text = "GO!";
            Time.timeScale = 1;
        }
        else if(remaining <= -1)
        {
            // make the timer disappear
            text.text = "";
            startCount = false;
            firstCount = false;
        }

    }

    private void changeColor(int _secondsSince)
    {
        int countLength;
        if (firstCount)
            countLength = firstCountLength;
        else
            countLength = generalCountLength;

        float diff = 255f / (float)countLength;
        text.color = new Color(255f - (diff * _secondsSince), diff * _secondsSince, 0f);
        Debug.Log(text.color.ToString());
    }

    public void requestCountdown()
    {
        startCount = true;
        timeSince = 0;
    }
}
