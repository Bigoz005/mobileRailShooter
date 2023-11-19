using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    [SerializeField]
    private GameObject gameoverCanvas;

    [SerializeField]
    private GameObject gameplayCanvas;

    [SerializeField]
    private Player player;

    private GameObject soundPlayer;
    private GameObject enemyPlayer;
    private GameObject musicManager;

    public void Awake()
    {
        soundPlayer = GameObject.FindGameObjectWithTag("SoundPlayer");
        enemyPlayer = GameObject.FindGameObjectWithTag("EnemyPlayer");
        musicManager = GameObject.FindGameObjectWithTag("MusicManager");
    }

    public void Replay()
    {
        soundPlayer.GetComponent<AudioSource>().Stop();
        enemyPlayer.GetComponent<AudioSource>().Stop();

        gameoverCanvas.SetActive(false);
        gameplayCanvas.SetActive(true);
        player.ResetScore();
        player.AddHealth(0);
        player.AddHealth(0);
        player.AddHealth(0);
        Time.timeScale = 1;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        
        if (musicManager.GetComponent<AudioSource>().clip.name.Equals("hardLevel"))
        {
            musicManager.GetComponent<AudioSource>().clip = musicManager.GetComponent<MusicManager>().MainMusic;
            musicManager.GetComponent<AudioSource>().Play();
        }
        else
        {
            musicManager.GetComponent<AudioSource>().UnPause();
        }

        soundPlayer.GetComponent<AudioSource>().Stop();
        enemyPlayer.GetComponent<AudioSource>().Stop();

        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }
}
