using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Shoot : MonoBehaviour
{
    [SerializeField]
    private Transform crosshair;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;
    public GameObject gun;
    private Zooming zoomController;
    string[] layerNames = { "RayCast", "Specials"};
    private float duration = 17.0f;
    private int points = 1000;

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
        points = points * (PlayerPrefs.GetInt("Difficulty", 0));
        /*crosshair = GameObject.FindGameObjectWithTag("Crosshair").transform;*/
        m_EventSystem = GetComponent<EventSystem>();
        zoomController = Camera.main.GetComponent<Zooming>();
        audioSource = GameObject.FindGameObjectWithTag("SoundPlayer").GetComponent<AudioSource>();
        enemyAudioSource = GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>();
        musicManager = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicManager>();
    }

    public void ShootRay()
    {
        Vector3 crosshairPosition = crosshair.localPosition;
        crosshairPosition.z = 100000000000f;
        crosshairPosition = Camera.main.ScreenToWorldPoint(crosshairPosition);

        gun.transform.LookAt(crosshair);
        Ray ray = new Ray(gun.transform.position, gun.transform.forward * 100000000);
        Debug.DrawLine(gun.transform.position, gun.transform.forward * 10000000, Color.green, 20);
        RaycastHit hit;

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
                }
                else
                {
                    hit.collider.gameObject.GetComponent<Enemy>().ResetAimlockAndCircles();
                    hit.collider.gameObject.GetComponent<Enemy>().StopAllGnomeCoroutines();
                    hit.collider.gameObject.GetComponent<Enemy>().enabled = false;
                    hit.collider.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    hit.collider.gameObject.transform.GetChild(2).gameObject.SetActive(false);
                    hit.collider.gameObject.transform.GetChild(3).gameObject.SetActive(false);
                }
                enemyAudioSource.Stop();
                Camera.main.gameObject.GetComponent<Player>().AddScore(points / 10);
                /*StopCoroutine(zoomController.ZoomOnEnemy());
                StopCoroutine(zoomController.Move());        */
            }

            if (hit.collider.CompareTag("ScorePowerUp"))
            {
                hit.collider.gameObject.SetActive(false);
                audioSource.clip = bonusClip;
                Camera.main.gameObject.GetComponent<Player>().AddScore(points);
            }

            if (hit.collider.CompareTag("BonusHealth"))
            {
                hit.collider.gameObject.SetActive(false);
                audioSource.clip = healthClip;
                Camera.main.gameObject.GetComponent<Player>().AddHealth(points);
            }


            if (hit.collider.CompareTag("PowerUp"))
            {
                hit.collider.gameObject.SetActive(false);
                musicManager.playPowerUpMusic();
                audioSource.clip = powerUpClip;
                StartCoroutine(powerUpDuration());
            }
        }
        
        audioSource.Play();
    }


    private IEnumerator powerUpDuration()
    {
        points = points * (PlayerPrefs.GetInt("Difficulty", 0) + 1);
        while (musicManager.powerUpOn)
        {
            if (duration <= 0)
            {
                points = points * PlayerPrefs.GetInt("Difficulty", 0);
                if(PlayerPrefs.GetInt("Difficulty", 0) == 2){
                    musicManager.playHardMusic();
                }
                else
                {
                    musicManager.playMainMusic();
                }
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
