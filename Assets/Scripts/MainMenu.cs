using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuCanvas;

    [SerializeField]
    private GameObject optionCanvas;

    [SerializeField]
    private GameObject highScoreText;

    private TextMeshProUGUI textMesh;
    const string SCORETEXT = "Score: ";

    public void Awake()
    {
        RefreshHighscore();
    }

    public void RefreshHighscore()
    {
        textMesh = highScoreText.GetComponent<TextMeshProUGUI>();
        textMesh.SetText(SCORETEXT + PlayerPrefs.GetInt("HighScore", 0));
    }

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
