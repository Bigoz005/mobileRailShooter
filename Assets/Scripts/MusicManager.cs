using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    AudioClip mainMusic;
    [SerializeField]
    AudioClip hardMusic;
    [SerializeField]
    AudioClip mediumMusic;
    [SerializeField]
    AudioClip powerUpMusic;

    private static MusicManager musicManagerInstance;
    public bool powerUpOn;

    public AudioClip MainMusic { get => mainMusic; set => mainMusic = value; }
    public AudioClip HardMusic { get => hardMusic; set => hardMusic = value; }

    public AudioClip MediumMusic { get => mediumMusic; set => mediumMusic = value; }
    public AudioClip PowerUpMusic { get => powerUpMusic; set => powerUpMusic = value; }

    void Awake()
    {
        if (musicManagerInstance == null)
        {
            musicManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (musicManagerInstance != this)
        {
            Destroy(gameObject);
        }
    }

    public void playMainMusic()
    {
        this.GetComponent<AudioSource>().clip = mainMusic;
        this.GetComponent<AudioSource>().Play();
        powerUpOn = false;
    }

    public void playMediumMusic()
    {
        this.GetComponent<AudioSource>().clip = mediumMusic;
        this.GetComponent<AudioSource>().Play();
    }

    public void playHardMusic()
    {
        this.GetComponent<AudioSource>().clip = hardMusic;
        this.GetComponent<AudioSource>().Play();
    }

    public void playPowerUpMusic()
    {
        this.GetComponent<AudioSource>().clip = powerUpMusic;
        this.GetComponent<AudioSource>().Stop();
        this.GetComponent<AudioSource>().Play();
        powerUpOn = true;
    }
}
