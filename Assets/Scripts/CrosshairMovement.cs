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
    private int tempPoints;
    private bool powerUpEnabled = false;

    private AudioClip shootClip;
    private AudioClip bonusClip;
    private AudioClip healthClip;
    private AudioClip powerUpClip;

    private float duration = 17.0f;
    private int points = 1000;
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
        points = points * (PlayerPrefs.GetInt("Difficulty", 0) + 1);
        tempPoints = points;

        if (PlayerPrefs.GetInt("Controls") == 1)
        {
            touchControlEnabled = true;
            controlsScoreDividor = 1;
            joystick.transform.parent.parent.gameObject.SetActive(false);
            shootButton.SetActive(false);
            self.GetComponent<Image>().enabled = false;
        }
        else
        {
            touchControlEnabled = false;
            joystick.transform.parent.gameObject.SetActive(true);
            shootButton.SetActive(true);
            self.GetComponent<Image>().enabled = true;
        }
    }

    void Update()
    {
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
                        Debug.DrawLine(cam.transform.position, ray.direction * 10000000, Color.red, 20);

                }

            }
        }
    }

    private void checkShoot(Ray ray)
    {
        RaycastHit hit;
        if (gameplayCanvas.activeSelf)
        {
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
                    }
                    enemyAudioSource.Stop();
                    Camera.main.gameObject.GetComponent<Player>().AddScore(points / 10 / controlsScoreDividor);
                }

                if (hit.collider.CompareTag("ScorePowerUp"))
                {
                    hit.collider.gameObject.SetActive(false);
                    audioSource.clip = bonusClip;
                    Camera.main.gameObject.GetComponent<Player>().AddScore(points / controlsScoreDividor);
                }

                if (hit.collider.CompareTag("BonusHealth"))
                {
                    hit.collider.gameObject.SetActive(false);
                    audioSource.clip = healthClip;
                    Camera.main.gameObject.GetComponent<Player>().AddHealth(points / controlsScoreDividor);
                }


                if (hit.collider.CompareTag("PowerUp"))
                {
                    hit.collider.gameObject.SetActive(false);
                    musicManager.playPowerUpMusic();
                    audioSource.clip = powerUpClip;
                    points = tempPoints;
                    duration = 17.0f;
                    if (!powerUpEnabled)
                    {
                        StartCoroutine(powerUpDuration());
                    }
                    else
                    {
                        musicManager.powerUpOn = true;
                        StopCoroutine(powerUpDuration());
                        StartCoroutine(powerUpDuration());
                    }
                }
            }

            audioSource.Play();
        }
    }

    private IEnumerator powerUpDuration()
    {
        powerUpEnabled = true;
        points = points * (PlayerPrefs.GetInt("Difficulty", 0) + 2);
        while (musicManager.powerUpOn)
        {
            if (duration <= 0)
            {
                powerUpEnabled = false;
                points = tempPoints * (PlayerPrefs.GetInt("Difficulty", 0) + 1);
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
    }

    private void Countdown()
    {
        duration -= 1;
    }

}
