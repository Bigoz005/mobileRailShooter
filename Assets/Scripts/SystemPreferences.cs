using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemPreferences : MonoBehaviour
{
    bool isPaused = false;
    public bool IsPaused { get => isPaused; set => isPaused = value; }
}
