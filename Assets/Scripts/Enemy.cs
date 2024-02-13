using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using System.Threading.Tasks;
public class Enemy : MonoBehaviour
{
    protected GameObject explosion;
    protected float duration;
    protected float time = 0;
    protected float TIME_TO_ATTACK = 1;
    private GameObject aimlock;
    private GameObject aimCircle1;
    private GameObject aimCircle2;
    private Material aimlockMaterial;
    [SerializeField] protected Zooming zoomController;
    [SerializeField] protected GameObject objectToWatch;
    protected int index;
    protected Transform originalSpecialElementTransform;

    [SerializeField] protected AnimationCurve curve;
    [SerializeField] protected AudioClip explosionClip;

    protected AudioSource audioSource;

    protected Vector3 startingPos;
    private Color startingAimlockMaterialColor;
    private Transform startingTransformAimlock;
    private Transform startingTransformCircle1;
    private Transform startingTransformCircle2;

    [SerializeField] public List<GameObject> specialElements;

    public float _Time { get => time; set => time = value; }

    public float _TIME_TO_ATTACK { get => TIME_TO_ATTACK; }
    private void OnEnable()
    {
        audioSource = GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>();
        if (PlayerPrefs.GetInt("Difficulty", 0) != 2)
        {
            GameObject.FindGameObjectWithTag("SoundPlayer").GetComponent<AudioSource>().pitch = 1.5f;
            audioSource.pitch = 1f;
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

        time = 0;       

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
        specialElements[index].gameObject.GetComponent<MeshRenderer>().enabled = true;
        specialElements[index].SetActive(true);
        originalSpecialElementTransform = specialElements[index].transform;

        zoomController.SetVariables(Camera.main, objectToWatch, this.gameObject, Camera.main.transform.GetChild(1).GetComponent<Camera>());

        StartCoroutine(zoomController.ZoomOnEnemy());
        StartCoroutine(zoomController.Move());
        CountdownToAttack();
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
        StartCoroutine(zoomController.MoveBack());
        StartCoroutine(zoomController.ZoomOutEnemy());
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
                gameObject.GetComponent<MeshRenderer>().enabled = true;
                gameObject.transform.GetChild(6).gameObject.SetActive(false);
            }
            Countdown();
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }

    private async void CountdownToAttack()
    {
        try
        {
            while (time <= TIME_TO_ATTACK)
            {
                while (Time.timeScale == 0)
                {
                    await Task.Delay(333);
                }

                if (time == TIME_TO_ATTACK - 0.5f && enabled)
                {
                    audioSource.clip = explosionClip;
                    audioSource.Play();
                }

                if (time == TIME_TO_ATTACK)
                {
                    Attack();
                }

                TimeCount();
                await Task.Delay(1000);
            }
            await Task.Yield();
        }
        catch (MissingReferenceException e)
        {
            audioSource.Stop();
            await Task.Yield();
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
            float timeFactor = 3f;
            bool wasYellow = false;

            while (time <= TIME_TO_ATTACK && enabled)
            {
                while (Time.timeScale == 0)
                {
                    yield return new WaitForSeconds(0.25f);
                }

                if (aimlock.transform.localScale.x >= 10)
                {
                    aimlock.transform.localScale -= scaleChange * 30 * timeFactor;
                }

                if (aimCircle1 != null)
                {
                    aimCircle1.transform.localScale -= (scaleChange * 550 * timeFactor / 2f);
                    if (aimCircle1.transform.localScale.x <= 0)
                    {
                        aimCircle1.SetActive(false);
                    }
                }

                if (aimCircle2 != null)
                {
                    aimCircle2.transform.localScale -= (scaleChange * 800 * timeFactor / 2f);

                    if (aimCircle2.transform.localScale.x <= 0)
                    {
                        aimCircle2.SetActive(false);
                    }
                }

                aimlock.transform.Rotate(0f, 5 * timeFactor * 10, 0f);

                if (wasYellow)
                {
                    aimlockMaterial.color = new Color(aimlockMaterial.color.r, aimlockMaterial.color.g - timeFactor / 100f, aimlockMaterial.color.b);
                }
                else
                {
                    aimlockMaterial.color = new Color(aimlockMaterial.color.r + timeFactor / 200f, aimlockMaterial.color.g, aimlockMaterial.color.b);
                    if (aimlockMaterial.color.r >= 1 && aimlockMaterial.color.g >= 1)
                    {
                        wasYellow = true;
                    }
                }
                yield return new WaitForSeconds(0.03f);
            }
            yield return null;   
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
        yield return null;
    }

    protected void Countdown()
    {
        duration -= 1;
    }
    protected void TimeCount()
    {
        time += 0.25f;
    }

    public void StopAllGnomeCoroutines()
    {
        StopCoroutine(Shaking());
        StopCoroutine(CountdownToExtinction());
        StopCoroutine(AimlockController());
    }

    protected void ResetSpecialItem()
    {
        specialElements[index].SetActive(false);
        specialElements[index].transform.localPosition = originalSpecialElementTransform.localPosition;
    }
}
