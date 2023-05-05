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

    void Update()
    {
        if(healthSys.health <= 0 || defusal.HasFailed())
        {
            Cursor.visible = true;

            //show failure text and buttons
            screenText.text = "GAME OVER!";
            screenText.color = Color.red;
            screenText.enabled = true;
            UI.SetActive(true);
        } else if (defusal.GetCompletion() && !defusal.HasFailed())
        {
            Cursor.visible = true;

            //show success text and buttons
            screenText.text = "SUCCESS!";
            screenText.color = Color.green;
            screenText.enabled = true;
            UI.SetActive(true);
        }
    }
}
