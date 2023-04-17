using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{
    public GameObject UI;
    public PlayerHealth healthSys;
    public BombDefusal defusal;
    [SerializeField]
    TMP_Text screenText;
    

    void Start()
    {
        //screenText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if(healthSys.health <= 0 || defusal.HasFailed())
        {
            //show failure text and buttons
            screenText.text = "GAME OVER!";
            screenText.enabled = true;
            UI.SetActive(true);
        } else if (defusal.GetCompletion() && !defusal.HasFailed())
        {
            //show success text and buttons
            screenText.text = "SUCCESS!";
            screenText.enabled = true;
            UI.SetActive(true);
        }
    }
}
