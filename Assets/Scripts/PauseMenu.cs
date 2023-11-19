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

    private GameObject musicManager;
    private GameObject soundPlayer;
    private GameObject enemyPlayer;

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
        musicManager.GetComponent<AudioSource>().UnPause();
        Camera.main.gameObject.GetComponent<SystemPreferences>().IsPaused = false;
        SceneManager.LoadScene("MainMenuScene");
    }
}
