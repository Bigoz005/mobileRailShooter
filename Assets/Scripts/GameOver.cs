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

    public void Awake()
    {
        soundPlayer = GameObject.FindGameObjectWithTag("SoundPlayer");
        enemyPlayer = GameObject.FindGameObjectWithTag("EnemyPlayer");
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
        soundPlayer.GetComponent<AudioSource>().Stop();
        enemyPlayer.GetComponent<AudioSource>().Stop();

        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }
}
