using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

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
    [SerializeField] private GameObject comicToggle;
    [SerializeField] private GameObject retroToggle;
    [SerializeField] private GameObject holoToggle;
    [SerializeField] private GameObject drawToggle;
    [SerializeField] private GameObject text;
    [SerializeField] private GameObject gnome1;
    [SerializeField] private GameObject gnome2;
    public TextMeshProUGUI username;

    [SerializeField] private Material[] skyboxMaterials;
    [SerializeField] private Material[] toonMaterials;
    [SerializeField] private Texture2D[] baseTextures;
    [SerializeField] private Texture2D[] comicTextures;

    public void Awake()
    {

        musicManager = GameObject.FindGameObjectWithTag("MusicManager");
        soundPlayer = GameObject.FindGameObjectWithTag("SoundPlayer");
        enemyPlayer = GameObject.FindGameObjectWithTag("EnemyPlayer");

        sliderFps.GetComponent<Slider>().value = PlayerPrefs.GetInt("FPS", 3);
        sliderMusic.GetComponent<Slider>().value = PlayerPrefs.GetInt("MusicVolume", 100);
        sliderSFX.GetComponent<Slider>().value = PlayerPrefs.GetInt("SFXVolume", 100);
        
        if (SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
        {
            sliderDifficulty.GetComponent<Slider>().value = PlayerPrefs.GetInt("Difficulty", 1);
            foreach (Material mat in toonMaterials)
            {
                DisableMaterialsPropetrties(mat, mat.shader);
            }

            comicToggle.GetComponent<Toggle>().isOn = false;
            drawToggle.GetComponent<Toggle>().isOn = false;
            retroToggle.GetComponent<Toggle>().isOn = false;
            holoToggle.GetComponent<Toggle>().isOn = false;


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

            if ((PlayerPrefs.GetInt("DrawShader", 1) == 0))
            {
                drawToggle.GetComponent<Toggle>().isOn = true;
            }
            else
            {
                drawToggle.GetComponent<Toggle>().isOn = false;
            }

            if ((PlayerPrefs.GetInt("RetroShader", 1) == 0))
            {
                retroToggle.GetComponent<Toggle>().isOn = true;
            }
            else
            {
                retroToggle.GetComponent<Toggle>().isOn = false;
            }

            if ((PlayerPrefs.GetInt("HoloShader", 1) == 0)) 
            {
                holoToggle.GetComponent<Toggle>().isOn = true;
            } else 
            {
                holoToggle.GetComponent<Toggle>().isOn = false;
            }

            foreach (Material mat in toonMaterials)
            {
                if ((bool)comicToggle.GetComponent<Toggle>().isOn)
                {
                    EnableComic(mat, mat.shader);
                }
                else if ((bool)drawToggle.GetComponent<Toggle>().isOn)
                {
                    EnableDraw(mat, mat.shader);
                }
                else if ((bool)retroToggle.GetComponent<Toggle>().isOn)
                {
                    EnableRetro(mat, mat.shader);
                } 
                else if ((bool)holoToggle.GetComponent<Toggle>().isOn) {
                    EnableHolo(mat, mat.shader);
                }
            }
            checkSkybox();
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
            PlayerPrefs.SetInt("DrawShader", 1);
            PlayerPrefs.SetInt("RetroShader", 1);
            PlayerPrefs.SetInt("HoloShader", 1);
            holoToggle.GetComponent<Toggle>().isOn = false;
            drawToggle.GetComponent<Toggle>().isOn = false;
            retroToggle.GetComponent<Toggle>().isOn = false;

            foreach (Material mat in toonMaterials)
            {
                Shader shader = mat.shader;
                EnableComic(mat, shader);
            }
        }
        else
        {
            PlayerPrefs.SetInt("ComicShader", 1);
        }

        checkSkybox();
        PlayerPrefs.Save();
    }

    public void SetRetroMaterials()
    {
        bool retroType = (bool)retroToggle.GetComponent<Toggle>().isOn;

        if (retroType)
        {
            PlayerPrefs.SetInt("RetroShader", 0);
            PlayerPrefs.SetInt("ComicShader", 1);
            PlayerPrefs.SetInt("DrawShader", 1);
            PlayerPrefs.SetInt("HoloShader", 1);
            holoToggle.GetComponent<Toggle>().isOn = false;
            comicToggle.GetComponent<Toggle>().isOn = false;
            drawToggle.GetComponent<Toggle>().isOn = false;

            foreach (Material mat in toonMaterials)
            {
                Shader shader = mat.shader;
                EnableRetro(mat, shader);
            }
        }
        else
        {
            PlayerPrefs.SetInt("RetroShader", 1);
        }
        checkSkybox();
        PlayerPrefs.Save();
    }

    public void SetDrawMaterials()
    {
        bool drawType = (bool)drawToggle.GetComponent<Toggle>().isOn;

        if (drawType)
        {
            PlayerPrefs.SetInt("DrawShader", 0);
            PlayerPrefs.SetInt("RetroShader", 1);
            PlayerPrefs.SetInt("ComicShader", 1);
            PlayerPrefs.SetInt("HoloShader", 1);
            holoToggle.GetComponent<Toggle>().isOn = false;
            comicToggle.GetComponent<Toggle>().isOn = false;
            retroToggle.GetComponent<Toggle>().isOn = false;

            foreach (Material mat in toonMaterials)
            {
                Shader shader = mat.shader;
                EnableDraw(mat, shader);
            }
        }
        else
        {
            PlayerPrefs.SetInt("DrawShader", 1);
        }
        checkSkybox();
        PlayerPrefs.Save();
    }

    public void SetHoloMaterials() {
        bool holoType = (bool)holoToggle.GetComponent<Toggle>().isOn;
        Debug.Log("OOOO : " + holoType);
        if (holoType) {
            PlayerPrefs.SetInt("DrawShader", 1);
            PlayerPrefs.SetInt("RetroShader", 1);
            PlayerPrefs.SetInt("ComicShader", 1);
            PlayerPrefs.SetInt("HoloShader", 0);
            drawToggle.GetComponent<Toggle>().isOn = false;
            comicToggle.GetComponent<Toggle>().isOn = false;
            retroToggle.GetComponent<Toggle>().isOn = false;

            foreach (Material mat in toonMaterials) {
                Shader shader = mat.shader;
                EnableHolo(mat, shader);
            }
        } else {
            PlayerPrefs.SetInt("HoloShader", 1);
        }
        checkSkybox();
        PlayerPrefs.Save();
    }

    public void checkSkybox()
    {
        if ((bool)drawToggle.GetComponent<Toggle>().isOn || (bool)retroToggle.GetComponent<Toggle>().isOn || (bool)comicToggle.GetComponent<Toggle>().isOn || (bool)holoToggle.GetComponent<Toggle>().isOn)
        {
            RenderSettings.skybox = skyboxMaterials[1];
            if ((bool)drawToggle.GetComponent<Toggle>().isOn)
            {
                RenderSettings.skybox.SetColor("_Tint", new(1, 1, 1, 1));
            }
            else{
                RenderSettings.skybox.SetColor("_Tint", new(0, 0.05f, 1, 1));
            }
        }
        else
        {
            RenderSettings.skybox = skyboxMaterials[0];
            foreach (Material mat in toonMaterials)
            {
                Shader shader = mat.shader;
                DisableMaterialsPropetrties(mat, shader);
            }
        }
    }

    public void EnableRetro(Material mat, Shader shader)
    {
        Shader.EnableKeyword("_retro_toggle");
        Shader.DisableKeyword("_draw_toggle");
        Shader.DisableKeyword("_comic_toggle");
        Shader.DisableKeyword("_holo_toggle");
        mat.SetFloat("_retro_toggle", 1);
        mat.SetFloat("_comic_toggle", 0);
        mat.SetFloat("_holo_toggle", 0);
        mat.SetFloat("_draw_toggle", 0);
        mat.SetFloat("_ZWrite", 0); // W³¹czenie zapisu do bufora Z
        mat.SetInt("_Cull", 1);
    }

    public void EnableHolo(Material mat, Shader shader) {
        Shader.DisableKeyword("_retro_toggle");
        Shader.DisableKeyword("_draw_toggle");
        Shader.DisableKeyword("_comic_toggle");
        Shader.EnableKeyword("_holo_toggle");
        mat.SetFloat("_comic_toggle", 0);
        mat.SetFloat("_draw_toggle", 0);
        mat.SetFloat("_retro_toggle", 0);
        mat.SetFloat("_holo_toggle", 1);
        mat.SetFloat("_ZWrite", 0); // Wy³¹czenie zapisu do bufora Z
        mat.SetInt("_Cull", 0);

    }


    public void EnableComic(Material mat, Shader shader)
    {
        Shader.DisableKeyword("_draw_toggle");
        Shader.DisableKeyword("_retro_toggle");
        Shader.EnableKeyword("_comic_toggle");
        Shader.DisableKeyword("_holo_toggle");
        mat.SetFloat("_comic_toggle", 1);
        mat.SetFloat("_retro_toggle", 0);
        mat.SetFloat("_draw_toggle", 0);
        mat.SetFloat("_holo_toggle", 0);
        mat.SetFloat("_ZWrite", 0); // W³¹czenie zapisu do bufora Z       
        mat.SetInt("_Cull", 1);
    }

    public void EnableDraw(Material mat, Shader shader)
    {
        Shader.EnableKeyword("_draw_toggle");
        Shader.DisableKeyword("_retro_toggle");
        Shader.DisableKeyword("_comic_toggle");
        Shader.DisableKeyword("_holo_toggle");
        mat.SetFloat("_holo_toggle", 0);
        mat.SetFloat("_draw_toggle", 1);
        mat.SetFloat("_retro_toggle", 0);
        mat.SetFloat("_comic_toggle", 0);
        mat.SetFloat("_ZWrite",0); // W³¹czenie zapisu do bufora Z
        mat.SetInt("_Cull",1);
    }

    public void DisableMaterialsPropetrties(Material mat, Shader shader)
    {
        Shader.DisableKeyword("_draw_toggle");
        Shader.DisableKeyword("_retro_toggle");
        Shader.DisableKeyword("_comic_toggle");
        Shader.DisableKeyword("_holo_toggle");
        mat.SetFloat("_holo_toggle", 0);
        mat.SetFloat("_draw_toggle", 0);
        mat.SetFloat("_retro_toggle", 0);
        mat.SetFloat("_comic_toggle", 0);
        mat.SetFloat("_ZWrite", 0); // W³¹czenie zapisu do bufora Z
        mat.SetInt("_Cull", 1);
    }
}
