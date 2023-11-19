using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class EnemyHard : MonoBehaviour
{
    private GameObject explosion;
    private float duration;
    private float time = 0;
    private float TIME_TO_ATTACK = 1.0f;
    private Zooming zoomController;
    private int index;
    private Transform originalSpecialElementTransform;

    private bool start = false;
    [SerializeField]
    public AnimationCurve curve;
    [SerializeField]
    public AudioClip explosionClip;

    private AudioSource audioSource;

    Vector3 startingPos;

    [SerializeField]
    private List<GameObject> specialElements;

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
        index = Random.Range(1, 3);
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;
        startingPos.z = transform.position.z;
        explosion = this.gameObject.transform.GetChild(0).gameObject;
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        transform.LookAt(Camera.main.transform);
        zoomController = Camera.main.GetComponent<Zooming>();
        zoomController.SetEnemy(this.gameObject);
        time = 0;

        audioSource = GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>();

        explosion.SetActive(false);
        duration = explosion.GetComponent<ParticleSystem>().main.duration - 1;

        int specialIndex = 0;
        switch (index % 3)
        {
            case 0:
                specialIndex = Random.Range(0, 3);
                break;
            case 1:
                specialIndex = Random.Range(4, 7);
                break;
            case 2:
                specialIndex = Random.Range(8, 11);
                break;
        }

        index = specialIndex;
        specialElements[index].SetActive(true);
        originalSpecialElementTransform = specialElements[index].transform;

        StartCoroutine(zoomController.ZoomOnEnemy());
        StartCoroutine(zoomController.Move());
        StartCoroutine(CountdownToAttack());
        StartCoroutine(Shaking());
    }

    private void Attack()
    {
        gameObject.layer = 0;
        explosion.SetActive(true);
        Camera.main.GetComponentInChildren<Player>().GetHit();
        if (Camera.main.GetComponentInChildren<Player>().GetHealth() != 0)
        {
            CameraShaker.Instance.ShakeOnce(3f, 3f, 0.34f, 0.34f);
        }
        explosion.GetComponent<ParticleSystem>().Play();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(CountdownToExtinction());
    }

    private IEnumerator CountdownToExtinction()
    {
        ResetSpecialItem();

        while (duration >= 0)
        {
            if (duration == 0)
            {
                gameObject.SetActive(false);
                this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            Countdown();
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator CountdownToAttack()
    {
        while (time <= TIME_TO_ATTACK)
        {
            if (time == TIME_TO_ATTACK - 1.0f)
            {
                audioSource.clip = explosionClip;
                audioSource.Play();
            }

            Debug.Log(time);
            Debug.Log(TIME_TO_ATTACK);
            if (time == TIME_TO_ATTACK - 0.5f)
            {
                Attack();
                StopCoroutine(CountdownToAttack());
                /*StartCoroutine(zoomController.ZoomOutEnemy());
                StartCoroutine(zoomController.MoveBack());*/
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
            float strength = curve.Evaluate(time*4 / TIME_TO_ATTACK);
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
        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        this.gameObject.SetActive(false);
        StopCoroutine(Shaking());
        StopCoroutine(CountdownToAttack());
        StopCoroutine(CountdownToExtinction());
        StopCoroutine(CameraShake());
    }

    private IEnumerator CameraShake()
    {
        Vector3 originalPos = Camera.main.transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < 1)
        {
            float x = Random.Range(-1f, 1f) * 3;
            float y = Random.Range(-1f, 1f) * 3;

            Camera.main.transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.fixedDeltaTime / 2;

            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }

    private void ResetSpecialItem()
    {
        specialElements[index].SetActive(false);
        specialElements[index].transform.localPosition = originalSpecialElementTransform.localPosition;
    }
}
