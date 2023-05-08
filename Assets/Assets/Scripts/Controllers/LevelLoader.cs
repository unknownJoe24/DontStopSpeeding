using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    public void PlayGame()
    {
        SceneManager.LoadScene("Level_MainGame_001");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Leaderboard()
    {
        SceneManager.LoadScene("Level_Leaderboard");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Level_MainMenu");
    }

    public void Upgrades()
    {
        SceneManager.LoadScene("Level_Upgrades");
    }
}
