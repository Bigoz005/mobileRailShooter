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

    private bool start = false;
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

    public float _Time { get => time; set => time = value; }

    public float _TIME_TO_ATTACK { get => TIME_TO_ATTACK; }
    private void OnEnable()
    {
        Debug.Log("OnEnable " + this.gameObject.name);
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;
        startingPos.z = transform.position.z;
        explosion = this.gameObject.transform.GetChild(0).gameObject;
        aimlock = this.gameObject.transform.GetChild(1).gameObject;
        aimCircle1 = this.gameObject.transform.GetChild(2).gameObject;
        aimCircle2 = this.gameObject.transform.GetChild(3).gameObject;
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
        if (explosion.GetComponent<ParticleSystem>().isPlaying) { 
            explosion.GetComponent<ParticleSystem>().Stop();
        }
        if (!explosion.GetComponent<ParticleSystem>().isPlaying)
        {
            explosion.GetComponent<ParticleSystem>().Play();
        }
        explosion.SetActive(false);

        StartCoroutine(zoomController.ZoomOnEnemy());
        StartCoroutine(zoomController.Move());
        StartCoroutine(CountdownToAttack());
        StartCoroutine(Shaking());
        StartCoroutine(AimlockController());
    }

    private void Attack()
    {                
        gameObject.layer = 0;
        aimlock.SetActive(false);
        StopCoroutine(AimlockController());
        Camera.main.GetComponentInChildren<Player>().GetHit();
        if(Camera.main.GetComponentInChildren<Player>().GetHealth() != 0) { 
            CameraShaker.Instance.ShakeOnce(3f, 3f, 0.34f, 0.34f);
        }
        explosion.gameObject.SetActive(true);
      
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        ResetAimlockAndCircles();

        StartCoroutine(CountdownToExtinction());
    }

    private IEnumerator CountdownToExtinction()
    {
        float tempDuration = duration - 1;
        
        
        while (duration >= 0)
        {
            if(duration == tempDuration)
            {
                explosion.SetActive(false);
            }

            if (duration == 0)
            {
                gameObject.SetActive(false);
                StopAllGnomeCoroutines();
            }
            Countdown();
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator CountdownToAttack()
    {
        Debug.Log("CountdownToAttack");
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

        bool wasYellow = false;

        while (time <= TIME_TO_ATTACK)
        {
            if (aimlock.transform.localScale.x >= 10)
            {
                aimlock.transform.localScale -= scaleChange * 50;
            }

            if (aimCircle1 != null)
            {
                aimCircle1.transform.localScale -= (scaleChange * 95);
                if (aimCircle1.transform.localScale.x <= 0)
                {
                    aimCircle1.SetActive(false);
                }
            }

            if (aimCircle2 != null)
            {
                aimCircle2.transform.localScale -= (scaleChange * 170);
                if (aimCircle2.transform.localScale.x <= 0)
                {
                    aimCircle2.SetActive(false);
                }
            }

            aimlock.transform.Rotate(0f, 75 * Time.fixedDeltaTime, 0f);

            if (wasYellow)
            {
                aimlockMaterial.color = new Color(aimlockMaterial.color.r, aimlockMaterial.color.g - Time.fixedDeltaTime * 0.35f, aimlockMaterial.color.b);
            }
            else
            {
                aimlockMaterial.color = new Color(aimlockMaterial.color.r + Time.fixedDeltaTime * 0.15f, aimlockMaterial.color.g, aimlockMaterial.color.b);
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

        while (time <= TIME_TO_ATTACK)
        {
            float strength = curve.Evaluate(time / TIME_TO_ATTACK);
            transform.position = startPostition + Random.insideUnitSphere * strength;
            yield return new WaitUntil(() => Camera.main.gameObject.GetComponent<SystemPreferences>().IsPaused == false );
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
}
