using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CrosshairMovement : MonoBehaviour
{

    [SerializeField]
    GameObject joystick;
    [SerializeField]
    GameObject self;
    [SerializeField]
    GameObject shootButton;
    [SerializeField]
    GameObject gameplayCanvas;


    private Camera cam;
    private Vector3 previousPosition = new Vector3(0, 0, 0);
    private AudioSource audioSource;
    private AudioSource enemyAudioSource;
    private MusicManager musicManager;
    string[] layerNames = { "RayCast", "Specials" };
    private int controlsScoreDividor = 5;
    private float previousTime;
    private float actualTime;

    private AudioClip shootClip;
    private AudioClip bonusClip;
    private AudioClip healthClip;
    private AudioClip powerUpClip;

    private float duration = 17.0f;
    private int points;
    private bool touchControlEnabled;

    private void Start()
    {
        cam = Camera.main;
        audioSource = GameObject.FindGameObjectWithTag("SoundPlayer").GetComponent<AudioSource>();
        enemyAudioSource = GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>();
        musicManager = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicManager>();
        shootClip = transform.GetComponent<Shoot>().shootClip;
        bonusClip = transform.GetComponent<Shoot>().bonusClip;
        healthClip = transform.GetComponent<Shoot>().healthClip;
        powerUpClip = transform.GetComponent<Shoot>().powerUpClip;
        points = Camera.main.GetComponent<Player>().GetPoints();
        points = points * (PlayerPrefs.GetInt("Difficulty", 0) + 1);
        previousTime = Time.deltaTime;
        actualTime = previousTime;

        if (PlayerPrefs.GetInt("Controls") == 1)
        {
            touchControlEnabled = true;
            joystick.transform.parent.parent.gameObject.SetActive(false);
            shootButton.SetActive(false);
            self.GetComponent<Image>().enabled = false;
        }
        else
        {
            touchControlEnabled = false;
            controlsScoreDividor = 1;
            joystick.transform.parent.gameObject.SetActive(true);
            shootButton.SetActive(true);
            self.GetComponent<Image>().enabled = true;
        }
    }

    void Update()
    {
        actualTime = actualTime + Time.deltaTime;
        if (!touchControlEnabled)
        {
            if (previousPosition.x != joystick.transform.localPosition.x || previousPosition.y != joystick.transform.localPosition.y)
            {
                previousPosition = joystick.transform.localPosition;
                self.transform.localPosition = new Vector3(previousPosition.x * 5.5f, previousPosition.y * 4f, previousPosition.z);
            }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    checkShoot(ray);

                    if (Physics.Raycast(ray))
                        Debug.DrawLine(cam.transform.position, ray.direction * 10000000000, Color.red, 20);

                }

            }
        }
    }

    private void checkShoot(Ray ray)
    {
        RaycastHit hit;

        if (!musicManager.powerUpOn)
        {
            points = Camera.main.GetComponent<Player>().GetPoints();
            points = points * (PlayerPrefs.GetInt("Difficulty", 0) + 1);
        }

        if (gameplayCanvas.activeSelf && (actualTime - previousTime) > 0.2f)
        {
            previousTime = actualTime;
            audioSource.clip = shootClip;
            if (Physics.SphereCast(ray, 0.1f, out hit, 10000000000, LayerMask.GetMask(layerNames)))
            {
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
                else
                if (hit.collider.CompareTag("ScorePowerUp"))
                {
                    hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    ParticleSystem particle = hit.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                    if (particle.isPlaying)
                    {
                        particle.Stop();
                    }
                    particle.Play();

                    audioSource.clip = bonusClip;
                    Camera.main.gameObject.GetComponent<Player>().AddScore(points / controlsScoreDividor);
                }
                else
                if (hit.collider.CompareTag("BonusHealth"))
                {
                    hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    ParticleSystem particle = hit.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                    if (particle.isPlaying)
                    {
                        particle.Stop();
                    }
                    particle.Play();

                    audioSource.clip = healthClip;
                    Camera.main.gameObject.GetComponent<Player>().AddHealth(points / 2 / controlsScoreDividor);
                }
                else
                if (hit.collider.CompareTag("PowerUp"))
                {
                    hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    ParticleSystem particle = hit.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                    if (particle.isPlaying)
                    {
                        particle.Stop();
                    }
                    particle.Play();

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
                else
                if (hit.collider.CompareTag("Target"))
                {
                    Camera.main.gameObject.GetComponent<Player>().AddScore(points / 10 / controlsScoreDividor * 2);
                }
            }
            else
            {
                Camera.main.gameObject.GetComponent<Player>().AddScore(-points / 10 / controlsScoreDividor);
            }

            audioSource.Play();
        }
    }

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
