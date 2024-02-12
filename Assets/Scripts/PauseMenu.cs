using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject optionCanvas;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private Player player;

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
        if (!soundPlayer.GetComponent<AudioSource>().isPlaying)
        {
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
        LoadSceneAsyncProcess("MainMenuScene");
    }

    private async void LoadSceneAsyncProcess(string sceneName)
    {
        _asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        player.interstitialAd.ShowAd();

        pauseCanvas.transform.GetChild(1).GetComponent<Button>().interactable = false;
        pauseCanvas.transform.GetChild(2).GetComponent<Button>().interactable = false;
        pauseCanvas.transform.GetChild(3).GetComponent<Button>().interactable = false;

        _asyncOperation.allowSceneActivation = false;

        while (!_asyncOperation.isDone)
        {
            if (_asyncOperation.progress >= 0.89)
            {
                pauseCanvas.transform.GetChild(1).GetComponent<Button>().interactable = true;
                pauseCanvas.transform.GetChild(2).GetComponent<Button>().interactable = true;
                pauseCanvas.transform.GetChild(3).GetComponent<Button>().interactable = true;
                _asyncOperation.allowSceneActivation = true;
            }
            await System.Threading.Tasks.Task.Yield();
        }

        await System.Threading.Tasks.Task.Yield();
    }
}
