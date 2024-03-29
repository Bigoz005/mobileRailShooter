using Dan.Main;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject optionCanvas;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject saveCanvas;
    [SerializeField] private Player player;

    private Color active = new Color(1, 1, 1, 1);
    private Color blocked = new Color(0.7f, 0.7f, 0.7f, 1);
    private GameObject musicManager;
    private GameObject soundPlayer;
    private GameObject enemyPlayer;
    /*private bool returnToMenu = false;*/

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

    private void TurnMenuMusic()
    {
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
        Time.timeScale = 1;
        Camera.main.gameObject.GetComponent<SystemPreferences>().IsPaused = false;
    }

    public void PrepareReturnToMenu()
    {
        if (player.GetScore() == 0)
        {
            BackToMainMenu();
        }
        else
        {
            if (player.GetScore() < PlayerPrefs.GetInt("Highscore"))
            {
                saveCanvas.transform.GetChild(0).gameObject.SetActive(false);
                saveCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Final score: " + player.GetScore() +"   Save?";
            }
            else
            {
                PlayerPrefs.SetInt("Highscore", player.GetScore());
                PlayerPrefs.Save();
            }
            
            pauseCanvas.SetActive(false);
            saveCanvas.SetActive(true);
        }
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
    private IEnumerator LoadSceneAsyncProcess(string sceneName)
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
            yield return null;
        }
        yield return null;
    }
}
