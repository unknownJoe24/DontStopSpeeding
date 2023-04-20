using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BombDefusal : MonoBehaviour
{
    // What keys should we choose from
    public KeyCode[] keys;

    // How many keys do they have to press and how fast
    public int numInputs;
    public float timer;

    // The UI
    public GameObject quickTimeUI;
    public GameObject sliderFill;

    // The field and sprites for the outcome of the keypresses
    [SerializeField]
    private Image outcomeSprite;
    public Sprite successSprite;
    public Sprite failureSprite;

    // A timer so the outcome is not always present
    private float outcomeFor;
    private float outcomeSince;

    // Is the diffusal process active
    private bool defusing;

    // What key is currently being prompted
    private KeyCode currKey;

    // How long has the prompt been up for
    private float timeUp;

    // How many prompts have been done
    private int done;

    // Has the defusal process been done before
    private bool completed;

    // Start is called before the first frame update
    void Start()
    {
        // initialize variables
        quickTimeUI.SetActive(false);
        defusing = false;
        done = 0;
        outcomeFor = 1f;
        completed = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool deadCurr = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().dead;

        // start defusal if the correct key is pressed and the bomb is/has not being/been defused
        if (!deadCurr && Input.GetButtonDown("Defuse Bomb") && !defusing && !completed)
        {
            defusing = true;
            currKey = getNewKey();
        }

        // continue defusal process if the bomb is currenlty being diffused
        if (!deadCurr && defusing)
            Defusal();
        else if (deadCurr)
            completed = true;
    }

    // get the next key that needs to be pressed
    KeyCode getNewKey()
    {
        // generate a random key to be pressing
        int keyIndex = Random.Range(0, keys.Length - 1);
        KeyCode newKey = keys[keyIndex];

        // when was the key generated
        timeUp = Time.time;

        // return the new key
        return newKey;
    }

    void lose()
    {

        // display an x next to the key
        outcomeSprite.sprite = failureSprite;
        outcomeSprite.color += new Color(1f, 1f, 1f, 1);
        outcomeSince = Time.time;
        
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().killPlayer();

        // the bomb diffusal process is not being diffused and (for the purpose of the code) has been diffused
        defusing = false;
        completed = true;
    }

    void checkInput()
    {
        // check to see if the user pressed the wrong key
        for (int i = 0; i < keys.Length; ++i)
        {
            if (keys[i] != currKey && Input.GetKeyDown(keys[i]))
            {
                lose();
            }
        }

        // did the user press the required key
        if (Input.GetKeyDown(currKey))
        {
            // display a checkmark next to the key
            outcomeSprite.sprite = successSprite;
            outcomeSprite.color += new Color(1f, 1f, 1f, 1);
            outcomeSince = Time.time;

            // the user still needs to press more keys
            if (++done < numInputs)
            {
                currKey = getNewKey();
            }
            // the user has pressed all keys
            else
            {
                // save the score
                GameObject.FindGameObjectWithTag("ScoreHandler").GetComponent<ScoreSystem>().saveScore();

                // make the quick time UI disappear
                quickTimeUI.SetActive(false);

                // the bomb is no longer being diffused and has been diffused
                defusing = false;
                completed = true;
            }
        }
    }

    void Defusal()
    {
        // check the input if the bomb is being diffused
        if(defusing)
            checkInput();

        // the user ran out of time
        if (Time.time - timeUp >= timer)
        {
            lose();
        }

        // no longer display the outcome
        if (Time.time - outcomeSince >= outcomeFor)
        {
            outcomeSprite.color = new Color(0f, 0f, 0f, 0f);
        }

        // display the key that needs to be pressed
        DisplayPrompt(currKey);
    }

    // display the key to be pressed
    void DisplayPrompt(KeyCode toPress)
    {
        if (defusing && !completed)
            Debug.Log(toPress);
        
        // make sure the quick time UI is appearing
        quickTimeUI.SetActive(true);

        // get the slider that shows how much time is left to press the key
        quickTimeUI.GetComponentInChildren<Slider>().value = (timer - (Time.time - timeUp)) / timer;

        // how long has the key been presented and decrease the slider accordingly
        float timeSince = Time.time - timeUp;
        sliderFill.GetComponentInChildren<Image>().color = new Color(timeSince, timer - timeSince, 0);
    }
}
