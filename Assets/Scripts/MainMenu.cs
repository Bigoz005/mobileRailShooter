using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuCanvas;

    [SerializeField]
    private GameObject optionCanvas;

    public void ShowOptions()
    {
        mainMenuCanvas.SetActive(false);
        optionCanvas.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
