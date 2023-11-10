using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHard : MonoBehaviour
{
    private GameObject explosion;
    private float duration;
    private float time = 0;
    private float TIME_TO_ATTACK = 1;
    private Zooming zoomController;

    private bool start = false;
    [SerializeField]
    public AnimationCurve curve;
    [SerializeField]
    public AudioClip explosionClip;

    private AudioSource audioSource;

    Vector3 startingPos;

    public float _Time { get => time; set => time = value; }

    public float _TIME_TO_ATTACK { get => TIME_TO_ATTACK; }

    void Awake()
    {
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;
        startingPos.z = transform.position.z;
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable " + this.gameObject.name);
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;
        startingPos.z = transform.position.z;
        explosion = this.gameObject.transform.GetChild(0).gameObject;
        
        transform.LookAt(Camera.main.transform);
        zoomController = Camera.main.GetComponent<Zooming>();
        zoomController.SetEnemy(this.gameObject);
        time = 0;

        audioSource = GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>();

        explosion.SetActive(false);
        duration = explosion.GetComponent<ParticleSystem>().main.duration - 1;
        explosion.GetComponent<ParticleSystem>().Stop();

        StartCoroutine(zoomController.ZoomOnEnemy());
        StartCoroutine(zoomController.Move());
        StartCoroutine(CountdownToAttack());
        StartCoroutine(Shaking());
    }

    /*private void Start()
    {

        transform.LookAt(Camera.main.transform);
        explosion = this.gameObject.transform.GetChild(0).gameObject;
        zoomController = Camera.main.GetComponent<Zooming>();
        zoomController.SetEnemy(this.gameObject);
        StartCoroutine(zoomController.ZoomOnEnemy());
        StartCoroutine(zoomController.Move());

        audioSource = GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>();

        explosion.SetActive(false);
        duration = explosion.GetComponent<ParticleSystem>().main.duration - 1;
        explosion.GetComponent<ParticleSystem>().Stop();
        StartCoroutine(CountdownToAttack());
        StartCoroutine(Shaking());
    }*/

    private void Attack()
    {
        gameObject.layer = 0;
        explosion.SetActive(true);
        Camera.main.GetComponentInChildren<Player>().GetHit();
        explosion.GetComponent<ParticleSystem>().Play();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(CountdownToExtinction());

    }

    private IEnumerator CountdownToExtinction()
    {
        while (duration >= 0)
        {
            if (duration == 0)
            {
                gameObject.SetActive(false);
            }
            Countdown();
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator CountdownToAttack()
    {
        while (time <= TIME_TO_ATTACK)
        {
            if (time == TIME_TO_ATTACK - 0.5f)
            {
                audioSource.clip = explosionClip;
                audioSource.Play();
            }

            if (time == TIME_TO_ATTACK)
            {
                Attack();
                StopCoroutine(CountdownToAttack());
                StartCoroutine(zoomController.ZoomOutEnemy());
                StartCoroutine(zoomController.MoveBack());
            }
            TimeCount();
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator Shaking()
    {

        Vector3 startPostition = transform.position;

        while (time <= TIME_TO_ATTACK)
        {
            float strength = curve.Evaluate(time / TIME_TO_ATTACK);
            transform.position = startPostition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPostition;
    }

    private void Countdown()
    {
        duration -= 1;
    }

    private void TimeCount()
    {
        time += 0.25f;
    }

    public void StopAllGnomeCoroutines()
    {
        StopCoroutine(Shaking());
        StopCoroutine(CountdownToAttack());
        StopCoroutine(CountdownToExtinction());
    }
}
