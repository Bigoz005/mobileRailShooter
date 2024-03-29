using System.Collections;
using Dan.Main;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{

    [SerializeField] private GameObject gameoverCanvas;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject saveCanvas;
    [SerializeField] private Player player;

    private Color active = new Color(1, 1, 1, 1);
    private Color blocked = new Color(0.7f, 0.7f, 0.7f, 1);
    private GameObject soundPlayer;
    private GameObject enemyPlayer;
    private GameObject musicManager;
    private AsyncOperation _asyncOperation;

    private bool returnToMenu = false;

    public void Awake()
    {
        soundPlayer = GameObject.FindGameObjectWithTag("SoundPlayer");
        enemyPlayer = GameObject.FindGameObjectWithTag("EnemyPlayer");
        musicManager = GameObject.FindGameObjectWithTag("MusicManager");
    }

    public void Replay()
    {
        if (player.GetScore() < PlayerPrefs.GetInt("Highscore"))
        {
            saveCanvas.transform.GetChild(0).gameObject.SetActive(false);
        }
        soundPlayer.GetComponent<AudioSource>().Stop();
        enemyPlayer.GetComponent<AudioSource>().Stop();
        returnToMenu = false;
        saveCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Final score: " + player.GetScore();
        gameoverCanvas.SetActive(false);
        saveCanvas.SetActive(true);
    }

    public void PrepareReturnToMenu()
    {
        if (player.GetScore() < PlayerPrefs.GetInt("Highscore"))
        {
            saveCanvas.transform.GetChild(0).gameObject.SetActive(false);
            saveCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Final score: " + player.GetScore() + "   Save?";
        }
        else
        {
            PlayerPrefs.SetInt("Highscore", player.GetScore());
            PlayerPrefs.Save();
        }
        returnToMenu = true;
        saveCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Final score: " + player.GetScore();
        gameoverCanvas.SetActive(false);
        saveCanvas.SetActive(true);
    }

    public void SaveAndUploadScore()
    {
        saveCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Uploading, please wait... ";
        SetButtonsInactive();
        int score = player.GetScore();
        

        Leaderboards.Gnomes.UploadNewEntry(PlayerPrefs.GetString("Username"), score, (msg) =>
        {
            Leaderboards.Gnomes.ResetPlayer();
            SetButtonsActive();

            BackToMainMenu();

        }, (error) =>
        {
            saveCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = error;
        });
    }

    private void SetButtonsActive()
    {
        saveCanvas.transform.GetChild(2).GetComponent<Button>().interactable = true;
        saveCanvas.transform.GetChild(2).GetComponent<Image>().color = active;
        saveCanvas.transform.GetChild(3).GetComponent<Button>().interactable = true;
        saveCanvas.transform.GetChild(3).GetComponent<Image>().color = active;
    }
    private void SetButtonsInactive()
    {
        saveCanvas.transform.GetChild(2).GetComponent<Button>().interactable = false;
        saveCanvas.transform.GetChild(2).GetComponent<Image>().color = blocked;
        saveCanvas.transform.GetChild(3).GetComponent<Button>().interactable = false;
        saveCanvas.transform.GetChild(2).GetComponent<Image>().color = blocked;
    }
    private void ActivateGameplay()
    {
        player.ShowInterAd();
        gameplayCanvas.SetActive(true);
        saveCanvas.SetActive(false);
        player.ResetScore();
        player.AddHealth(0);
        player.AddHealth(0);
        player.AddHealth(0);
        ((CrosshairMovement)gameplayCanvas.transform.GetChild(0).GetComponent<Button>().onClick.GetPersistentTarget(0)).turnOffPowerUp();
        player.isRewarded = false;
        Time.timeScale = 1;
    }

    public void BackToMainMenu()
    {
        if (returnToMenu)
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
        else
        {
            ActivateGameplay();
        }
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
