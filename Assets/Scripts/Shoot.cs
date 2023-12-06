using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Shoot : MonoBehaviour
{
    [SerializeField]
    private Transform crosshair;
    private EventSystem m_EventSystem;
    public GameObject gun;
    string[] layerNames = { "RayCast", "Specials" };
    private float duration = 17.0f;
    private int points;
    private int controlsScoreDividor = 5;
    private float previousTime;
    private float actualTime;

    private AudioSource audioSource;
    private AudioSource enemyAudioSource;
    private MusicManager musicManager;

    [SerializeField]
    public AudioClip shootClip;
    [SerializeField]
    public AudioClip bonusClip;
    [SerializeField]
    public AudioClip healthClip;
    [SerializeField]
    public AudioClip powerUpClip;

    // Start is called before the first frame update
    void Start()
    {
        points = Camera.main.GetComponent<Player>().GetPoints();
        points = points * (PlayerPrefs.GetInt("Difficulty", 0) + 1);
        m_EventSystem = GetComponent<EventSystem>();
        audioSource = GameObject.FindGameObjectWithTag("SoundPlayer").GetComponent<AudioSource>();
        enemyAudioSource = GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>();
        musicManager = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicManager>();
        if (!(PlayerPrefs.GetInt("Controls") == 1))
        {
            controlsScoreDividor = 1;
        }
        previousTime = Time.deltaTime;
        actualTime = previousTime;
    }

    private void Update()
    {
        actualTime = actualTime + Time.deltaTime;
    }

    public void ShootRay()
    {
        Vector3 crosshairPosition = crosshair.localPosition;
        crosshairPosition.z = 100000000000f;
        crosshairPosition = Camera.main.ScreenToWorldPoint(crosshairPosition);

        gun.transform.LookAt(crosshair);
        Ray ray = new Ray(gun.transform.position, gun.transform.forward * 10000000);
        Debug.DrawLine(gun.transform.position, gun.transform.forward * 10000000, Color.green, 20);
        RaycastHit hit;

        audioSource.clip = shootClip;
        if ((actualTime - previousTime) > 0.2f)
        {
            previousTime = actualTime;

            if (Physics.SphereCast(ray, 0.1f, out hit, 10000000000, LayerMask.GetMask(layerNames)))
            {
                if (!musicManager.powerUpOn)
                {
                    points = Camera.main.GetComponent<Player>().GetPoints();
                    points = points * (PlayerPrefs.GetInt("Difficulty", 0) + 1);
                }

                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    if (hit.collider.name.Contains("Hard"))
                    {
                        hit.collider.gameObject.GetComponent<EnemyHard>().StopAllGnomeCoroutines();
                        hit.collider.gameObject.GetComponent<EnemyHard>().enabled = false;
                        hit.collider.gameObject.transform.GetChild(3).gameObject.SetActive(true);
                        hit.collider.gameObject.tag = "Untagged";
                    }
                    else
                    {
                        hit.collider.gameObject.GetComponent<Enemy>().ResetAimlockAndCircles();
                        hit.collider.gameObject.GetComponent<Enemy>().StopAllGnomeCoroutines();
                        hit.collider.gameObject.GetComponent<Enemy>().enabled = false;
                        hit.collider.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                        hit.collider.gameObject.transform.GetChild(2).gameObject.SetActive(false);
                        hit.collider.gameObject.transform.GetChild(3).gameObject.SetActive(false);
                        hit.collider.gameObject.transform.GetChild(6).gameObject.SetActive(true);
                        hit.collider.gameObject.tag = "Untagged";
                    }
                    enemyAudioSource.Stop();
                    if (musicManager.powerUpOn)
                    {
                        Debug.Log(musicManager.powerUpOn);
                        Camera.main.gameObject.GetComponent<Player>().AddScore(2 * (points / 10 / controlsScoreDividor));
                    }
                    else
                    {
                        Camera.main.gameObject.GetComponent<Player>().AddScore(points / 10 / controlsScoreDividor);
                    }
                }

                if (hit.collider.CompareTag("ScorePowerUp"))
                {
                    hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;

                    ParticleSystem particle = hit.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                    if (particle.isPlaying)
                    {
                        particle.Stop();
                    }
                    particle.Play();

                    /*Debug.Log(hit.collider.gameObject.transform.GetChild(0).name + ": " + hit.collider.gameObject.transform.GetChild(0).gameObject.activeInHierarchy + "-play: " + particle.isPlaying);*/
                    /*StartCoroutine(effectDuration(hit.collider.gameObject.transform.GetChild(0).gameObject));*/

                    audioSource.clip = bonusClip;
                    Camera.main.gameObject.GetComponent<Player>().AddScore(points / controlsScoreDividor);
                }

                if (hit.collider.CompareTag("BonusHealth"))
                {
                    hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;

                    ParticleSystem particle = hit.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                    if (particle.isPlaying)
                    {
                        particle.Stop();
                    }
                    particle.Play();

                    /*Debug.Log(hit.collider.gameObject.transform.GetChild(0).name + ": " + hit.collider.gameObject.transform.GetChild(0).gameObject.activeInHierarchy + "-play: " + particle.isPlaying);*/
                    /*StartCoroutine(effectDuration(hit.collider.gameObject.transform.GetChild(0).gameObject));*/

                    audioSource.clip = healthClip;
                    Camera.main.gameObject.GetComponent<Player>().AddHealth(points / 2 / controlsScoreDividor);
                }


                if (hit.collider.CompareTag("PowerUp"))
                {
                    hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;

                    ParticleSystem particle = hit.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                    if (particle.isPlaying)
                    {
                        particle.Stop();
                    }
                    particle.Play();

                    /*Debug.Log(hit.collider.gameObject.transform.GetChild(0).name + ": " + hit.collider.gameObject.transform.GetChild(0).gameObject.activeInHierarchy + "-play: " + particle.isPlaying);*/
                    /*StartCoroutine(effectDuration(hit.collider.gameObject.transform.GetChild(0).gameObject));*/

                    musicManager.playPowerUpMusic();
                    audioSource.clip = powerUpClip;
                    duration = 17.0f;
                    if (!musicManager.powerUpOn)
                    {
                        StartCoroutine(powerUpDuration());
                    }
                    else
                    {
                        StopCoroutine(powerUpDuration());
                        StartCoroutine(powerUpDuration());
                    }
                }
            }

            audioSource.Play();
        }
    }

    /*private IEnumerator effectDuration(GameObject effect)
    {
        int i = 0;
        while (i < 1)
        {
            i++;
            yield return new WaitForSeconds(1);
        }
        effect.SetActive(false);
    }*/

    private IEnumerator powerUpDuration()
    {
        musicManager.powerUpOn = true;
        while (musicManager.powerUpOn)
        {
            if (duration <= 0)
            {
                if (PlayerPrefs.GetInt("Difficulty", 0) == 2)
                {
                    musicManager.playHardMusic();
                }
                else
                {
                    musicManager.playMainMusic();
                }
                musicManager.powerUpOn = false;
            }
            Countdown();
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }

    private void Countdown()
    {
        duration -= 1;
    }
}
