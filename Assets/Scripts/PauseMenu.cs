using Dan.Main;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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
        pauseCanvas.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText("Record score?");
        pauseCanvas.transform.GetChild(1).gameObject.SetActive(false);
        pauseCanvas.transform.GetChild(2).gameObject.SetActive(false);
        pauseCanvas.transform.GetChild(3).gameObject.SetActive(false);
        pauseCanvas.transform.GetChild(4).gameObject.SetActive(true);
        pauseCanvas.transform.GetChild(5).gameObject.SetActive(true);
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

    public void Save()
    {

        if (player.GetScore() > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", player.GetScore());
            PlayerPrefs.Save();
        }

        TurnMenuMusic();
        StartCoroutine(LoadSceneAsyncProcess("MainMenuScene"));
    }

    public void DontSave()
    {
        TurnMenuMusic();
        StartCoroutine(LoadSceneAsyncProcess("MainMenuScene"));
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
