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
    public TextMeshProUGUI buttonDisplay; 

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
        quickTimeUI.SetActive(false);
        defusing = false;
        done = 0;
        outcomeFor = 1f;
        completed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Defuse Bomb") && !defusing && !completed)
        {
            defusing = true;
            currKey = getNewKey();
        }

        if (defusing)
            Defusal();
    }

    // get the next key that needs to be pressed
    KeyCode getNewKey()
    {
        KeyCode newKey = keys[Random.Range(0, keys.Length - 1)];

        buttonDisplay.text = newKey.ToString();

        timeUp = Time.time;

        return newKey;
    }

    void lose()
    {
        Debug.Log("You Lose");

        // display an x next to the key
        outcomeSprite.sprite = failureSprite;
        outcomeSprite.color += new Color(1f, 1f, 1f, 1);
        outcomeSince = Time.time;

        GameObject.FindGameObjectWithTag("Player").GetComponent<VehicleBehaviour.WheelVehicle>().disableInput();

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
                Debug.Log("Wrong Key!");
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
                GameObject.FindGameObjectWithTag("ScoreHandler").GetComponent<ScoreSystem>().saveScore();
                Debug.Log("You win!");

                quickTimeUI.SetActive(false);
                defusing = false;
                completed = true;
            }
        }
    }

    void Defusal()
    {
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

        DisplayPrompt(currKey);
    }

    // display the key to be pressed
    void DisplayPrompt(KeyCode toPress)
    {
        if (defusing)
            Debug.Log(toPress);

        quickTimeUI.SetActive(true);

        quickTimeUI.GetComponentInChildren<Slider>().value = (timer - (Time.time - timeUp)) / timer;

        float timeSince = Time.time - timeUp;
        sliderFill.GetComponentInChildren<Image>().color = new Color(timeSince, timer - timeSince, 0);
    }
}
