using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Shoot : MonoBehaviour
{
    public float range = 10000f;
    private Transform crosshair;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;
    public GameObject gun;
    private Zooming zoomController;
    public LayerMask mask;

    private AudioSource audioSource;


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
        crosshair = GameObject.FindGameObjectWithTag("Crosshair").transform;
        m_EventSystem = GetComponent<EventSystem>();
        zoomController = Camera.main.GetComponent<Zooming>();
        audioSource = Camera.main.GetComponent<AudioSource>();
    }

    public void ShootRay()
    {
        Vector3 crosshairPosition = crosshair.localPosition;
        crosshairPosition.z = 100000000000f;
        crosshairPosition = Camera.main.ScreenToWorldPoint(crosshairPosition);

        gun.transform.LookAt(crosshair);
        Ray ray = new Ray(gun.transform.position, gun.transform.forward * 100000000);
        Debug.DrawRay(gun.transform.position, gun.transform.forward * 10000000, Color.green);
        RaycastHit hit;

        audioSource.clip = shootClip;
        if (Physics.Raycast(ray, out hit, 1000000, mask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
                Camera.main.gameObject.GetComponent<Player>().AddScore(100);
                StopCoroutine(zoomController.ZoomOnEnemy());
                StopCoroutine(zoomController.Move());
                StartCoroutine(zoomController.ZoomOutEnemy());
                StartCoroutine(zoomController.MoveBack());
            }

            if (hit.collider.CompareTag("ScorePowerUp"))
            {
                audioSource.clip = bonusClip;
                Destroy(hit.collider.gameObject);
                Camera.main.gameObject.GetComponent<Player>().AddScore(1000);
            }

            if (hit.collider.CompareTag("BonusHealth"))
            {
                audioSource.clip = healthClip;
                Destroy(hit.collider.gameObject);
                Camera.main.gameObject.GetComponent<Player>().AddHealth();
            }


            if (hit.collider.CompareTag("PowerUp"))
            {
                audioSource.clip = powerUpClip;
                Destroy(hit.collider.gameObject);
                Camera.main.gameObject.GetComponent<Player>().AddHealth();
            }
        }

        audioSource.Play();        
    }
}
