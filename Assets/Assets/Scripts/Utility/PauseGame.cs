using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour {

    public bool gamePaused = false;
    public GameObject pauseMenu;

	void Update () {
		if (Input.GetButtonDown("Cancel"))
        {
            if(gamePaused == false)
            {
                print("Game Paused");
                Time.timeScale = 0;
                gamePaused = true;
                Cursor.visible = true;
                pauseMenu.SetActive(true);
            }
            else
            {
                pauseMenu.SetActive(false);
                Cursor.visible = false;
                gamePaused = false;
                //Time.timeScale = 1;
                GameObject.Find("Countdown").GetComponent<StartCountdown>().requestCountdown();
            }
        }
	}

    public void ResumeGame()
    {
        Debug.Log("Resume Game");
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        gamePaused = false;
        GameObject.Find("Countdown").GetComponent<StartCountdown>().requestCountdown();
        //Time.timeScale = 1;
    }

    public void RestartLevel()
    {
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        gamePaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void QuitToMenu()
    {
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        gamePaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
