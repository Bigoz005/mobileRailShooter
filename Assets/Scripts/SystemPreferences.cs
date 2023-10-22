using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemPreferences : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS", 0);
    }
}
