using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_ControlObject : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject winMenu;

    public GameObject spawn;

    public int maxDestroyedBalls = 10;

    public int DestroyedBallCounter;
    public int Score;

    // Start is called before the first frame update
    void Start()
    {
        DestroyedBallCounter = 0;
        HidePauseMenu();
        HideWinMenu();
        gameOverMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (Time.timeScale == 0)
                HidePauseMenu();
            else
                ShowPauseMenu();
    }

    public void IncreaseDestroyedBallCounter(int count)
    {
        DestroyedBallCounter += count;
        Debug.Log(DestroyedBallCounter);
        if (DestroyedBallCounter >= maxDestroyedBalls)
        {
            Debug.Log("called");
            spawn.GetComponent<S_Route>().StopSpawning();
            ShowWinMenu();
        }
    }

    public void Resume() => HidePauseMenu();

    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void MainMenu() => SceneManager.LoadScene("MainMenu");

    public void ShowGameOverMenu()
    {
        gameOverMenu.SetActive(true);
        Time.timeScale = 0;
    }

    void HidePauseMenu()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    void HideWinMenu()
    {
        winMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowWinMenu()
    {
        winMenu.SetActive(true);
        Time.timeScale = 0;
    }
}
