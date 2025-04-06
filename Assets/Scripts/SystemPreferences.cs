using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemPreferences : MonoBehaviour
{
    bool isPaused = false;
    public bool IsPaused { get => isPaused; set => isPaused = value; }

    [SerializeField] private Material[] toonMaterials;
    [SerializeField] private Material skybox;

    public void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        Color color = new(1, 1, 1, 1);

        if (PlayerPrefs.GetInt("DrawShader", 1) == 0)
        {
            RenderSettings.skybox = skybox;
            RenderSettings.skybox.SetColor("_Tint", new(1, 1, 1, 1));

            switch (SceneManager.GetActiveScene().name)
            {
                case "EasyScene":
                    color = new(0.85f, 0.85f, 0.85f, 1);
                    break;
                case "MediumScene":
                    color = new(0.65f, 0.65f, 0.65f, 1);
                    break;
                case "HardScene":
                    color = new(0.36f, 0.36f, 0.36f, 1);
                    break;
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("RetroShader", 1) == 0 || PlayerPrefs.GetInt("ComicShader", 1) == 0 || PlayerPrefs.GetInt("HoloShader", 1) == 0)
            {
                RenderSettings.skybox = skybox;
                RenderSettings.skybox.SetColor("_Tint", new(0, 0.05f, 1, 1));
            }

            switch (SceneManager.GetActiveScene().name)
            {
                case "EasyScene":
                    color = new(0.7f, 1f, 0.7f, 1);
                    break;
                case "MediumScene":
                    color = new(0.7f, 0.5f, 0.5f, 1);
                    break;
                case "HardScene":
                    color = new(0.6f, 0.3f, 0.8f, 1);
                    break;
            }
        }


        foreach (Material mat in toonMaterials)
        {
            mat.color = color;
        }
    }
}