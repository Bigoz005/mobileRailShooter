using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
public class Enemy : MonoBehaviour
{
    private GameObject explosion;
    private float duration;
    private float time = 0;
    private float TIME_TO_ATTACK = 1;
    private GameObject aimlock;
    private GameObject aimCircle1;
    private GameObject aimCircle2;
    private Material aimlockMaterial;
    private Zooming zoomController;
    private int index;
    private Transform originalSpecialElementTransform;

    [SerializeField]
    public AnimationCurve curve;
    [SerializeField]
    public AudioClip explosionClip;

    private AudioSource audioSource;

    private Vector3 startingPos;
    private Color startingAimlockMaterialColor;
    private Transform startingTransformAimlock;
    private Transform startingTransformCircle1;
    private Transform startingTransformCircle2;

    [SerializeField]
    private List<GameObject> specialElements;

    public float _Time { get => time; set => time = value; }

    public float _TIME_TO_ATTACK { get => TIME_TO_ATTACK; }
    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("Difficulty", 0) != 2)
        {
            GameObject.FindGameObjectWithTag("SoundPlayer").GetComponent<AudioSource>().pitch = 1.5f;
            GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>().pitch = 1f;
        }

        index = Random.Range(1, 4);
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;
        startingPos.z = transform.position.z;
        explosion = this.gameObject.transform.GetChild(0).gameObject;
        aimlock = this.gameObject.transform.GetChild(1).gameObject;
        aimCircle1 = this.gameObject.transform.GetChild(2).gameObject;
        aimCircle2 = this.gameObject.transform.GetChild(3).gameObject;

        if (aimlock.activeInHierarchy == false)
            aimlock.SetActive(true);
        if (aimCircle1.activeInHierarchy == false)
            aimCircle1.SetActive(true);
        if (aimCircle2.activeInHierarchy == false)
            aimCircle2.SetActive(true);

        gameObject.GetComponent<MeshRenderer>().enabled = true;
        startingAimlockMaterialColor = aimlock.GetComponent<MeshRenderer>().material.color;
        startingTransformAimlock = aimlock.transform;
        startingTransformCircle1 = aimCircle1.transform;
        startingTransformCircle2 = aimCircle2.transform;

        transform.LookAt(Camera.main.transform);
        aimlock.SetActive(true);
        aimCircle1.SetActive(true);
        aimCircle2.SetActive(true);
        zoomController = Camera.main.GetComponent<Zooming>();
        zoomController.SetEnemy(this.gameObject);
        time = 0;

        audioSource = GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>();

        aimlockMaterial = aimlock.GetComponent<MeshRenderer>().material;
        duration = explosion.GetComponent<ParticleSystem>().main.duration - 1;
        if (explosion.GetComponent<ParticleSystem>().isPlaying)
        {
            explosion.GetComponent<ParticleSystem>().Stop();
        }
        if (!explosion.GetComponent<ParticleSystem>().isPlaying)
        {
            explosion.GetComponent<ParticleSystem>().Play();
        }
        explosion.SetActive(false);

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
        StartCoroutine(AimlockController());
    }

    private void Attack()
    {
        if (enabled)
        {
            gameObject.layer = 0;
            aimlock.SetActive(false);
            StopCoroutine(AimlockController());
            Camera.main.GetComponentInChildren<Player>().GetHit();
            if (Camera.main.GetComponentInChildren<Player>().GetHealth() != 0)
            {
                CameraShaker.Instance.ShakeOnce(3f, 3f, 0.34f, 0.34f);
            }
            explosion.gameObject.SetActive(true);
        }
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        ResetAimlockAndCircles();
        StartCoroutine(CountdownToExtinction());
    }

    public IEnumerator CountdownToExtinction()
    {
        float tempDuration = duration - 1;

        ResetSpecialItem();

        while (duration >= 0)
        {
            if (duration == tempDuration)
            {
                explosion.SetActive(false);
            }

            if (duration == 0)
            {
                gameObject.SetActive(false);
                StopAllGnomeCoroutines();
                this.gameObject.GetComponent<MeshRenderer>().enabled = true;
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
            if (time == TIME_TO_ATTACK - 0.5f && enabled)
            {
                audioSource.clip = explosionClip;
                audioSource.Play();
            }

            if (time == TIME_TO_ATTACK)
            {
                Attack();
                StopCoroutine(CountdownToAttack());
            }

            TimeCount();
            yield return new WaitForSeconds(1);
        }

    }

    public void ResetAimlockAndCircles()
    {
        aimlock.transform.position = startingTransformAimlock.position;
        aimlock.transform.localScale = startingTransformAimlock.localScale;
        aimlock.transform.rotation = startingTransformAimlock.rotation;
        aimlock.GetComponent<MeshRenderer>().material.color = startingAimlockMaterialColor;
        aimCircle1.transform.position = startingTransformCircle1.position;
        aimCircle1.transform.localScale = startingTransformCircle1.localScale;
        aimCircle1.transform.rotation = startingTransformCircle1.rotation;
        aimCircle2.transform.position = startingTransformCircle2.position;
        aimCircle2.transform.localScale = startingTransformCircle2.localScale;
        aimCircle2.transform.rotation = startingTransformCircle2.rotation;
    }

    private IEnumerator AimlockController()
    {
        Vector3 scaleChange = new Vector3(0.003f, 0.003f, 0.003f);
        float timeFactor = 30f;
        int fps = PlayerPrefs.GetInt("FPS", 0);

        switch (fps)
        {
            case 0:
                timeFactor = 150f;
                break;
            case 1:
                timeFactor = 100f;
                break;
            case 2:
                timeFactor = 60f;
                break;
            case 3:
                timeFactor = 30f;
                break;
        }

        bool wasYellow = false;

        while (time <= TIME_TO_ATTACK && enabled)
        {
            if (aimlock.transform.localScale.x >= 10)
            {
                aimlock.transform.localScale -= scaleChange * 30 * timeFactor / 10f;
            }

            if (aimCircle1 != null)
            {
                aimCircle1.transform.localScale -= (scaleChange * 120 * timeFactor / 30f);
                if (aimCircle1.transform.localScale.x <= 0)
                {
                    aimCircle1.SetActive(false);
                }
            }

            if (aimCircle2 != null)
            {
                aimCircle2.transform.localScale -= (scaleChange * 160 * timeFactor / 30f);
                if (aimCircle2.transform.localScale.x <= 0)
                {
                    aimCircle2.SetActive(false);
                }
            }

            aimlock.transform.Rotate(0f, 5*timeFactor * Time.fixedDeltaTime, 0f);

            if (wasYellow)
            {
                aimlockMaterial.color = new Color(aimlockMaterial.color.r, aimlockMaterial.color.g - Time.fixedDeltaTime * timeFactor/100f, aimlockMaterial.color.b);
            }
            else
            {
                aimlockMaterial.color = new Color(aimlockMaterial.color.r + Time.fixedDeltaTime * timeFactor/200f, aimlockMaterial.color.g, aimlockMaterial.color.b);
                if (aimlockMaterial.color.r >= 1 && aimlockMaterial.color.g >= 1)
                {
                    wasYellow = true;
                }
            }

            yield return new WaitUntil(() => Camera.main.gameObject.GetComponent<SystemPreferences>().IsPaused == false);
        }
    }
    private IEnumerator Shaking()
    {
        Vector3 startPostition = transform.position;

        while (time <= TIME_TO_ATTACK && enabled)
        {
            float strength = curve.Evaluate(time / TIME_TO_ATTACK);
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
        StopCoroutine(AimlockController());
        StopCoroutine(CountdownToAttack());
        StopCoroutine(CountdownToExtinction());
    }

    private void ResetSpecialItem()
    {
        specialElements[index].SetActive(false);
        specialElements[index].transform.localPosition = originalSpecialElementTransform.localPosition;
    }
}
