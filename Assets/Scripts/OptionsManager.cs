using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OptionsManager : MonoBehaviour
{
    private int fps;
    private int musicVolume;
    private int sfxVolume;
    private int difficulty;
    private int highScore;


    void Start()
    {
        GetFPS();
        GetMusicVolume();
        GetSFXVolume();
        GetDifficulty();
        GetHighScore();
    }

    public void SetFPS()
    {
        int fps = (int)this.gameObject.GetComponent<Slider>().value;
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

    public void SetMusicVolume(int musicVolume)
    {
        PlayerPrefs.SetInt("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(int sfxVolume)
    {
        PlayerPrefs.SetInt("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }
    public void Setdifficulty(int difficulty)
    {
        PlayerPrefs.SetInt("Difficulty", difficulty);
        PlayerPrefs.Save();
    }
    public void SetHighScore(int highscore)
    {
        PlayerPrefs.SetInt("HighScore", highscore);
        PlayerPrefs.Save();
    }

    public int GetFPS()
    {
        fps = PlayerPrefs.GetInt("FPS");
        return fps;
    }

    public int GetMusicVolume()
    {
        musicVolume = PlayerPrefs.GetInt("MusicVolume");
        return musicVolume;
    }

    public int GetSFXVolume()
    {
        sfxVolume = PlayerPrefs.GetInt("SFXVolume");
        return sfxVolume;
    }

    public int GetDifficulty()
    {
        difficulty = PlayerPrefs.GetInt("Difficulty");
        return difficulty;
    }

    public int GetHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore");
        return highScore;
    }

}
