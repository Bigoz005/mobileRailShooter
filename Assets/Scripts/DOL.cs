using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOL : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}


