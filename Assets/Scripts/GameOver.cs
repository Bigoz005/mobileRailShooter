using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private AsyncOperation _asyncOperation;

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

        soundPlayer.GetComponent<AudioSource>().Stop();
        enemyPlayer.GetComponent<AudioSource>().Stop();

        Time.timeScale = 1;

        StartCoroutine(LoadSceneAsyncProcess("MainMenuScene"));
    }

    private IEnumerator LoadSceneAsyncProcess(string sceneName)
    {
        // Begin to load the Scene you have specified.
        this._asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        gameoverCanvas.transform.GetChild(2).GetComponent<Button>().interactable = false;
        gameoverCanvas.transform.GetChild(3).GetComponent<Button>().interactable = false;
        // Don't let the Scene activate until you allow it to.
        this._asyncOperation.allowSceneActivation = false;

        while (!this._asyncOperation.isDone)
        {
            /*Debug.Log($"[scene]:{sceneName} [load progress]: {this._asyncOperation.progress}");*/

            if (this._asyncOperation.progress >= 0.89)
            {
                gameoverCanvas.transform.GetChild(2).GetComponent<Button>().interactable = true;
                gameoverCanvas.transform.GetChild(3).GetComponent<Button>().interactable = true;
                this._asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
        yield return null;
    }
}
