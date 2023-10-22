using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    AudioClip mainMusic;
    [SerializeField]
    AudioClip powerUpMusic;

    public bool powerUpOn;

    public void Update()
    {
        if (!this.GetComponent<AudioSource>().isPlaying)
        {
            playMainMusic();
        }
    }

    public void playMainMusic()
    {
        this.GetComponent<AudioSource>().clip = mainMusic;
        this.GetComponent<AudioSource>().Play();
        powerUpOn = false;
    }

    public void playPowerUpMusic()
    {
        this.GetComponent<AudioSource>().clip = powerUpMusic;
        this.GetComponent<AudioSource>().Play();
        powerUpOn = true;
    }
}
