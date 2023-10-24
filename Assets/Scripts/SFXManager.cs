using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private static SFXManager sfxManagerInstance;
    void Awake()
    {
        if (sfxManagerInstance == null)
        {
            sfxManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (sfxManagerInstance != this)
        {
            Destroy(gameObject);
        }
    }
}
