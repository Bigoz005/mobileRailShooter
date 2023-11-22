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

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("Difficulty", 0) == 2)
        {
            GameObject.FindGameObjectWithTag("SoundPlayer").GetComponent<AudioSource>().pitch = 0.5f;
            GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>().pitch = 0.75f;
        }

        index = Random.Range(1, 4);
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
        if (enabled)
        {
            gameObject.layer = 0;
            
            Camera.main.GetComponentInChildren<Player>().GetHit();
            if (Camera.main.GetComponentInChildren<Player>().GetHealth() != 0)
            {
                CameraShaker.Instance.ShakeOnce(3f, 3f, 0.34f, 0.34f);
            }
            explosion.SetActive(true);
        }
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(CountdownToExtinction());
    }

    public IEnumerator CountdownToExtinction()
    {
        ResetSpecialItem();

        while (duration >= 0)
        {
            if (duration == 0)
            {
                gameObject.SetActive(false);
                gameObject.transform.GetChild(3).gameObject.SetActive(false);
                gameObject.GetComponent<MeshRenderer>().enabled = true;
                StopCoroutine(zoomController.ZoomOnEnemy());
                StopCoroutine(zoomController.Move());
            }
            Countdown();
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator CountdownToAttack()
    {
        while (time <= TIME_TO_ATTACK)
        {
            if (time == TIME_TO_ATTACK - 1.0f && enabled)
            {
                audioSource.clip = explosionClip;
                audioSource.Play();
            }

            if (time == TIME_TO_ATTACK - 0.5f)
            {
                Attack();
                StopCoroutine(CountdownToAttack());
            }
            TimeCount();
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator Shaking()
    {

        Vector3 startPostition = transform.position;

        while (time <= TIME_TO_ATTACK && enabled)
        {
            float strength = curve.Evaluate(time * 4 / TIME_TO_ATTACK);
            transform.position = startPostition + Random.insideUnitSphere * strength;
            yield return new WaitUntil(() => Camera.main.gameObject.GetComponent<SystemPreferences>().IsPaused == false);
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
