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

    public void Replay()
    {
        gameoverCanvas.SetActive(false);
        gameplayCanvas.SetActive(true);
        player.AddHealth(0);
        player.AddHealth(0);
        player.AddHealth(0);
        Time.timeScale = 1;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }
}
