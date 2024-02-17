using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemPreferences : MonoBehaviour
{
    bool isPaused = false;
    [SerializeField]
    private Material[] materials;
    private Color baseColor = new(1, 1, 1, 1);
    public bool IsPaused { get => isPaused; set => isPaused = value; }

    public void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        foreach (Material mat in materials)
        {
            mat.color = baseColor;
        }
    }
}
