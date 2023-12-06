using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject pauseCanvas;

    [SerializeField]
    private GameObject optionCanvas;

    [SerializeField]
    private GameObject gameplayCanvas;

    private GameObject musicManager;
    private GameObject soundPlayer;
    private GameObject enemyPlayer;

    private AsyncOperation _asyncOperation;
    public void Awake()
    {
        musicManager = GameObject.FindGameObjectWithTag("MusicManager");
        soundPlayer = GameObject.FindGameObjectWithTag("SoundPlayer");
        enemyPlayer = GameObject.FindGameObjectWithTag("EnemyPlayer");
    }

    public void ShowOptions()
    {
        musicManager.GetComponent<AudioSource>().UnPause();
        soundPlayer.GetComponent<AudioSource>().Pause();
        enemyPlayer.GetComponent<AudioSource>().Pause();
        pauseCanvas.SetActive(false);
        optionCanvas.SetActive(true);
    }

    public void Continue()
    {
        musicManager.GetComponent<AudioSource>().UnPause();
        if (!soundPlayer.GetComponent<AudioSource>().isPlaying) { 
        soundPlayer.GetComponent<AudioSource>().UnPause();
        }
        if (!enemyPlayer.GetComponent<AudioSource>().isPlaying)
        {
            enemyPlayer.GetComponent<AudioSource>().UnPause();
        }
        Camera.main.gameObject.GetComponent<SystemPreferences>().IsPaused = false;
        pauseCanvas.SetActive(false);
        gameplayCanvas.SetActive(true);
        Time.timeScale = 1;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        musicManager.GetComponent<MusicManager>().powerUpOn = false;
        if (musicManager.GetComponent<MusicManager>().HardMusic.name.Equals(musicManager.GetComponent<AudioSource>().clip.name) || musicManager.GetComponent<MusicManager>().PowerUpMusic.name.Equals(musicManager.GetComponent<AudioSource>().clip.name))
        {
            musicManager.GetComponent<AudioSource>().clip = musicManager.GetComponent<MusicManager>().MainMusic;
            musicManager.GetComponent<AudioSource>().Play();
        }
        else
        {
            musicManager.GetComponent<AudioSource>().UnPause();
        }

        Camera.main.gameObject.GetComponent<SystemPreferences>().IsPaused = false;
        StartCoroutine(LoadSceneAsyncProcess("MainMenuScene"));
    }

    private IEnumerator LoadSceneAsyncProcess(string sceneName)
    {
        // Begin to load the Scene you have specified.
        this._asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        pauseCanvas.transform.GetChild(1).GetComponent<Button>().interactable = false;
        pauseCanvas.transform.GetChild(2).GetComponent<Button>().interactable = false;
        pauseCanvas.transform.GetChild(3).GetComponent<Button>().interactable = false;
        // Don't let the Scene activate until you allow it to.
        this._asyncOperation.allowSceneActivation = false;

        while (!this._asyncOperation.isDone)
        {
            /*Debug.Log($"[scene]:{sceneName} [load progress]: {this._asyncOperation.progress}");*/

            if (this._asyncOperation.progress >= 0.89)
            {
                pauseCanvas.transform.GetChild(1).GetComponent<Button>().interactable = true;
                pauseCanvas.transform.GetChild(2).GetComponent<Button>().interactable = true;
                pauseCanvas.transform.GetChild(3).GetComponent<Button>().interactable = true;
                this._asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        yield return null;
    }
}
