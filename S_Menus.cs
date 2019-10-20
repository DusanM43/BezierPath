using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_Menus : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject Levels;

    // Start is called before the first frame update
    void Start()
    {
        MainMenu.SetActive(true);
        Levels.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Quit() => Application.Quit();

    public void ShowLevels()
    {
        MainMenu.SetActive(false);
        Levels.SetActive(true);
    }

    public void HideLevels()
    {
        MainMenu.SetActive(true);
        Levels.SetActive(false);
    }

    public void LoadLevel(string SceneName) => SceneManager.LoadScene(SceneName);
}
