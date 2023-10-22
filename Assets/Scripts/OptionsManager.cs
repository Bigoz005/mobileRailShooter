using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public void SetFPS(int fps)
    {
        PlayerPrefs.SetInt("FPS", fps);
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
        fps = PlayerPrefs.GetInt("FPS", 30);
        return fps;
    }

    public int GetMusicVolume()
    {
        musicVolume = PlayerPrefs.GetInt("MusicVolume", 50);
        return musicVolume;
    }

    public int GetSFXVolume()
    {
        sfxVolume = PlayerPrefs.GetInt("SFXVolume", 50);
        return sfxVolume;
    }

    public int GetDifficulty()
    {
        difficulty = PlayerPrefs.GetInt("Difficulty", 0);
        return difficulty;
    }

    public int GetHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        return highScore;
    }
    
}
