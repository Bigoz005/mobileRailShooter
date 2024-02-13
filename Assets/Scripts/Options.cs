using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject optionCanvas;
    [SerializeField] private GameObject creditsCanvas;
    [SerializeField] private AudioClip sfxClip;

    private GameObject musicManager;
    private GameObject soundPlayer;
    private GameObject enemyPlayer;

    [SerializeField] private GameObject sliderFps;
    [SerializeField] private GameObject sliderDifficulty;
    [SerializeField] private GameObject sliderMusic;
    [SerializeField] private GameObject sliderSFX;
    [SerializeField] private GameObject touchToggle;
    [SerializeField] private GameObject text;
    [SerializeField] private GameObject gnome1;
    [SerializeField] private GameObject gnome2;
    public TextMeshProUGUI username;

    public void Awake()
    {

        musicManager = GameObject.FindGameObjectWithTag("MusicManager");
        soundPlayer = GameObject.FindGameObjectWithTag("SoundPlayer");
        enemyPlayer = GameObject.FindGameObjectWithTag("EnemyPlayer");

        sliderFps.GetComponent<Slider>().value = PlayerPrefs.GetInt("FPS", 3);
        sliderDifficulty.GetComponent<Slider>().value = PlayerPrefs.GetInt("Difficulty", 1);
        sliderMusic.GetComponent<Slider>().value = PlayerPrefs.GetInt("MusicVolume", 100);
        sliderSFX.GetComponent<Slider>().value = PlayerPrefs.GetInt("SFXVolume", 100);
        if (SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
        {
            if ((PlayerPrefs.GetInt("Controls", 1) == 0))
            {
                touchToggle.GetComponent<Toggle>().isOn = true;
            }
            else
            {

                touchToggle.GetComponent<Toggle>().isOn = false;
            }
        }
        GetFPS();
        if (username != null)
        {
            username.text = "Username: " + PlayerPrefs.GetString("Username", "----");
        }
    }

    public void ShowMenu()
    {
        if (!SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
        {
            musicManager.GetComponent<AudioSource>().Pause();
        }
        soundPlayer.GetComponent<AudioSource>().Pause();
        enemyPlayer.GetComponent<AudioSource>().Pause();
        mainMenuCanvas.SetActive(true);
        optionCanvas.SetActive(false);
        if (gnome1 != null && gnome2 != null)
        {
            gnome1.SetActive(true);
            gnome2.SetActive(true);
        }
    }
    public void ShowCredits()
    {
        creditsCanvas.SetActive(true);
        optionCanvas.SetActive(false);
    }

    public void ShowOptions()
    {
        creditsCanvas.SetActive(false);
        optionCanvas.SetActive(true);
        if (gnome1 != null && gnome2 != null)
        {
            gnome1.SetActive(false);
            gnome2.SetActive(false);
        }
    }

    public void ResetScore()
    {
        PlayerPrefs.SetInt("HighScore", 0);
        PlayerPrefs.Save();
        if (text != null)
        {
            text.GetComponent<TextMeshProUGUI>().text = "HighScore: 0";
        }
    }

    public void SetFPS()
    {
        int fps = (int)sliderFps.GetComponent<Slider>().value;

        switch (fps)
        {
            case 0:
                {
                    PlayerPrefs.SetInt("FPS", 0);
                    Application.targetFrameRate = 30;
                    break;
                }
            case 1:
                {
                    PlayerPrefs.SetInt("FPS", 1);
                    Application.targetFrameRate = 60;
                    break;
                }
            case 2:
                {
                    PlayerPrefs.SetInt("FPS", 2);
                    Application.targetFrameRate = 90;
                    break;
                }
            case 3:
                {
                    PlayerPrefs.SetInt("FPS", 3);
                    Application.targetFrameRate = 120;
                    break;
                }
            default:
                GetFPS();
                break;
        }
        PlayerPrefs.Save();
    }

    public void GetFPS()
    {
        switch (PlayerPrefs.GetInt("FPS"))
        {
            case 0:
                {
                    Application.targetFrameRate = 30;
                    break;
                }
            case 1:
                {
                    Application.targetFrameRate = 60;
                    break;
                }
            case 2:
                {
                    Application.targetFrameRate = 90;
                    break;
                }
            case 3:
                {
                    Application.targetFrameRate = 120;
                    break;
                }
        }
    }

    public void SetMusic()
    {
        int volume = (int)sliderMusic.GetComponent<Slider>().value;
        musicManager.GetComponent<AudioSource>().volume = volume / 100f;
        PlayerPrefs.SetInt("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFX()
    {
        int volume = (int)sliderSFX.GetComponent<Slider>().value;
        soundPlayer.GetComponent<AudioSource>().volume = volume / 100f;
        enemyPlayer.GetComponent<AudioSource>().volume = volume / 100f;

        soundPlayer.GetComponent<AudioSource>().clip = sfxClip;

        if (volume != PlayerPrefs.GetInt("SFXVolume"))
        {
            soundPlayer.GetComponent<AudioSource>().Play();
        }
        PlayerPrefs.SetInt("SFXVolume", volume);
        PlayerPrefs.Save();
    }
    public void SetDifficulty()
    {
        int difficulty = (int)sliderDifficulty.GetComponent<Slider>().value;
        PlayerPrefs.SetInt("Difficulty", difficulty);
        PlayerPrefs.Save();
    }

    public void SetControls()
    {
        bool shootType = (bool)touchToggle.GetComponent<Toggle>().isOn;
        if (shootType)
        {
            PlayerPrefs.SetInt("Controls", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Controls", 1);
        }
        PlayerPrefs.Save();
    }
}
