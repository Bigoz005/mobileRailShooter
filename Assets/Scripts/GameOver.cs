using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{

    [SerializeField] private GameObject gameoverCanvas;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private Player player;

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
        player.isRewarded = false;
        ((CrosshairMovement)gameplayCanvas.transform.GetChild(0).GetComponent<Button>().onClick.GetPersistentTarget(0)).turnOffPowerUp();
        Time.timeScale = 1;
    }

    public void BackToMainMenu()
    {
        player.ShowInterAd();

        musicManager.GetComponent<MusicManager>().powerUpOn = false;
        if (musicManager.GetComponent<MusicManager>().HardMusic.name.Equals(musicManager.GetComponent<AudioSource>().clip.name) || musicManager.GetComponent<MusicManager>().PowerUpMusic.name.Equals(musicManager.GetComponent<AudioSource>().clip.name) || musicManager.GetComponent<MusicManager>().MediumMusic.name.Equals(musicManager.GetComponent<AudioSource>().clip.name))
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
        this._asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        player.interstitialAd.ShowAd();
        player.interstitialAd.wasPlayedOnGameOver = false;
        gameoverCanvas.transform.GetChild(2).GetComponent<Button>().interactable = false;
        gameoverCanvas.transform.GetChild(3).GetComponent<Button>().interactable = false;

        this._asyncOperation.allowSceneActivation = false;

        while (!this._asyncOperation.isDone)
        {
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
