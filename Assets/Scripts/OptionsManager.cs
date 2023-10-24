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
        Debug.Log(fps);
        if (fps == 0)
        {
            PlayerPrefs.SetInt("FPS", 30);
            Application.targetFrameRate = 30;
            Debug.Log(Application.targetFrameRate);
        }
        else
        {
            PlayerPrefs.SetInt("FPS", 60);
            Application.targetFrameRate = 60;
            Debug.Log(Application.targetFrameRate);
        }
        PlayerPrefs.Save();
        Debug.Log(PlayerPrefs.GetInt("FPS"));
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
