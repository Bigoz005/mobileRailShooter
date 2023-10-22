using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuCanvas;

    [SerializeField]
    private GameObject optionCanvas;

    [SerializeField]
    private GameObject sliderFps;
    [SerializeField]
    private GameObject sliderDifficulty;
    [SerializeField]
    private GameObject sliderMusic;
    [SerializeField]
    private GameObject sliderSFX;

    public void Awake()
    {
        sliderFps.GetComponent<Slider>().value = PlayerPrefs.GetInt("FPS", 0);
        sliderDifficulty.GetComponent<Slider>().value = PlayerPrefs.GetInt("Difficulty", 0);
        sliderMusic.GetComponent<Slider>().value = PlayerPrefs.GetInt("MuiscVolume", 0);
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
    }

    public void SetFPS(int newValue)
    {
        if (newValue == 0)
        {
            PlayerPrefs.SetInt("FPS", 30);
            Application.targetFrameRate = 30;
        }
        else
        {
            PlayerPrefs.SetInt("FPS", 60);
            Application.targetFrameRate = 60;
        }
        PlayerPrefs.Save();
    }

    public void SetMusic(int newValue)
    {
        PlayerPrefs.SetInt("MusicVolume", newValue);
        PlayerPrefs.Save();
    }
    public void SetSFX(int newValue)
    {
        PlayerPrefs.SetInt("SFXVolume", newValue);
        PlayerPrefs.Save();
    }
    public void SetDifficulty(int newValue)
    {
        PlayerPrefs.SetInt("Difficulty", newValue);
        PlayerPrefs.Save();
    }
}
