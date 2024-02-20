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


    [SerializeField] private Skybox proceduralSkybox;
    [SerializeField] private GameObject sliderFps;
    [SerializeField] private GameObject sliderDifficulty;
    [SerializeField] private GameObject sliderMusic;
    [SerializeField] private GameObject sliderSFX;
    [SerializeField] private GameObject touchToggle;
    [SerializeField] private GameObject comicToggle;
    [SerializeField] private GameObject text;
    [SerializeField] private GameObject gnome1;
    [SerializeField] private GameObject gnome2;
    public TextMeshProUGUI username;

    [SerializeField] private Material[] toonMaterials;
    [SerializeField] private Texture2D[] baseTextures;
    [SerializeField] private Texture2D[] comicTextures;

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

            if ((PlayerPrefs.GetInt("ComicShader", 1) == 0))
            {
                comicToggle.GetComponent<Toggle>().isOn = true;
            }
            else
            {
                comicToggle.GetComponent<Toggle>().isOn = false;
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

    public void SetComicMaterials()
    {
        bool comicType = (bool)comicToggle.GetComponent<Toggle>().isOn;
        
        if (comicType)
        {
            PlayerPrefs.SetInt("ComicShader", 0);
            toonMaterials[0].mainTexture = comicTextures[0];
            toonMaterials[14].mainTexture = comicTextures[1];

            toonMaterials[1].mainTexture = null;
            toonMaterials[2].mainTexture = null;
            
            toonMaterials[3].mainTexture = null;
            toonMaterials[4].mainTexture = null;
            toonMaterials[5].mainTexture = null;
            toonMaterials[6].mainTexture = null;
            toonMaterials[7].mainTexture = null;

            toonMaterials[8].mainTexture = comicTextures[2];
            toonMaterials[9].mainTexture = comicTextures[2];

            toonMaterials[11].color = new(0.7f, 0, 0, 1);
            toonMaterials[11].color = new(0.5f, 1, 1, 1);
            RenderSettings.skybox.SetFloat("_AtmosphereThickness", 0.75f);
        }
        else
        {
            PlayerPrefs.SetInt("ComicShader", 1);
            toonMaterials[0].mainTexture = baseTextures[3];
            toonMaterials[14].mainTexture = baseTextures[4];
            
            toonMaterials[8].mainTexture = baseTextures[2];
            toonMaterials[9].mainTexture = baseTextures[2];
            
            toonMaterials[1].mainTexture = baseTextures[0];
            toonMaterials[2].mainTexture = baseTextures[0];
            
            toonMaterials[3].mainTexture = baseTextures[1];
            toonMaterials[4].mainTexture = baseTextures[1];
            toonMaterials[5].mainTexture = baseTextures[1];
            toonMaterials[6].mainTexture = baseTextures[1];
            toonMaterials[7].mainTexture = baseTextures[1];

            toonMaterials[11].color = new(1f, 0, 0, 1);
            toonMaterials[11].color = new(0.8f, 1, 1, 1);

            RenderSettings.skybox.SetFloat("_AtmosphereThickness", 0.35f);

        }        

        PlayerPrefs.Save();
    }
}
