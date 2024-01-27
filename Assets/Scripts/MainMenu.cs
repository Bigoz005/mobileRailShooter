using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject optionCanvas;
    [SerializeField] private GameObject highScoreText;

    private TextMeshProUGUI textMesh;
    private const string SCORETEXT = "Score: ";

    private AsyncOperation _asyncOperation;
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
        int difficulty = PlayerPrefs.GetInt("Difficulty");
        string sceneName = "";
        switch (difficulty)
        {
            case 0:
                sceneName = "EasyScene";
                break;
            case 1:
                sceneName = "MediumScene";
                break;
            case 2:
                GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>().clip = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicManager>().HardMusic;
                GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>().Play();
                sceneName = "HardScene";
                break;
        }
        StartCoroutine(LoadSceneAsyncProcess(sceneName));
    }

    private IEnumerator LoadSceneAsyncProcess(string sceneName)
    {
        // Begin to load the Scene you have specified.
        this._asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        mainMenuCanvas.transform.GetChild(1).GetComponent<Button>().interactable = false;
        mainMenuCanvas.transform.GetChild(2).GetComponent<Button>().interactable = false;
        mainMenuCanvas.transform.GetChild(3).GetComponent<Button>().interactable = false;

        // Don't let the Scene activate until you allow it to.
        this._asyncOperation.allowSceneActivation = false;

        while (!this._asyncOperation.isDone)
        {
            /*Debug.Log($"[scene]:{sceneName} [load progress]: {this._asyncOperation.progress}");*/

            if (this._asyncOperation.progress >= 0.89)
            {
                mainMenuCanvas.transform.GetChild(1).GetComponent<Button>().interactable = true;
                mainMenuCanvas.transform.GetChild(2).GetComponent<Button>().interactable = true;
                mainMenuCanvas.transform.GetChild(3).GetComponent<Button>().interactable = true;
                this._asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        yield return null;
    }
}
