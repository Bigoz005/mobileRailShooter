using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject pauseCanvas;

    [SerializeField]
    private GameObject optionCanvas;

    [SerializeField]
    private GameObject gameplayCanvas;

    public void ShowOptions()
    {
        pauseCanvas.SetActive(false);
        optionCanvas.SetActive(true);
    }

    public void Continue()
    {
        pauseCanvas.SetActive(false);
        gameplayCanvas.SetActive(true);
        Time.timeScale = 1;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }
}
