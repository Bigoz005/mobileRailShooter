using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuCanvas;
    [SerializeField]
    private GameObject optionCanvas;

    private GameObject musicManager;
    private GameObject soundPlayer;
    private GameObject enemyPlayer;


    [SerializeField]
    private GameObject sliderFps;
    [SerializeField]
    private GameObject sliderDifficulty;
    [SerializeField]
    private GameObject sliderMusic;
    [SerializeField]
    private GameObject sliderSFX;

    [SerializeField]
    private GameObject text;

    public void Awake()
    {
        musicManager = GameObject.FindGameObjectWithTag("MusicManager");
        soundPlayer = GameObject.FindGameObjectWithTag("SoundPlayer");
        enemyPlayer = GameObject.FindGameObjectWithTag("EnemyPlayer");

        sliderFps.GetComponent<Slider>().value = PlayerPrefs.GetInt("FPS", 0);
        sliderDifficulty.GetComponent<Slider>().value = PlayerPrefs.GetInt("Difficulty", 0);
        sliderMusic.GetComponent<Slider>().value = PlayerPrefs.GetInt("MusicVolume", 0);
        sliderSFX.GetComponent<Slider>().value = PlayerPrefs.GetInt("SFXVolume", 0);
    }

    public void ShowMenu()
    {
        mainMenuCanvas.SetActive(true);
        optionCanvas.SetActive(false);
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
        }
        PlayerPrefs.Save();
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
        PlayerPrefs.SetInt("SFXVolume", volume);
        PlayerPrefs.Save();
    }
    public void SetDifficulty()
    {
        int volume = (int)sliderDifficulty.GetComponent<Slider>().value;
        PlayerPrefs.SetInt("Difficulty", volume);
        PlayerPrefs.Save();
    }
}
