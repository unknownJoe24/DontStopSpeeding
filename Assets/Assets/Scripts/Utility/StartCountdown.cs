using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartCountdown : MonoBehaviour
{
    public bool startCount = true;  // tells the script to count down or not
    public int countLength = 5;     // how long does the countdown last
    private bool firstCount;        // is this the countodwn for the beginning of the game
    private float timeSince;        // how long has it been since the countdown started

    private TMP_Text text;          // text itself

    // Start is called before the first frame update
    void Start()
    {
        firstCount = true;
        timeSince = 0;
        text = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startCount)
            countdown();
    }

    private void countdown()
    {
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
        }

    }

    private void changeColor(int _secondsSince)
    {
        float diff = 255f / (float)countLength;
        text.color = new Color(255f - (diff * _secondsSince), diff * _secondsSince, 0f);
    }

    public void requestCountdown()
    {
        startCount = true;
        timeSince = 0;
    }
}
