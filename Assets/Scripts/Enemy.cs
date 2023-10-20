using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject explosion;
    private float duration;
    private float time = 0;
    private const int TIME_TO_ATTACK = 8;
    private GameObject aimlock;
    private GameObject aimCircle1;
    private GameObject aimCircle2;
    private Material aimlockMaterial;

    private bool start = false;
    [SerializeField]
    AnimationCurve curve;

    Vector3 startingPos;

    void Awake()
    {
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;
        startingPos.z = transform.position.z;
    }

    private void Start()
    {
        transform.LookAt(Camera.main.transform);
        explosion = this.gameObject.transform.GetChild(0).gameObject;
        aimlock = this.gameObject.transform.GetChild(1).gameObject;
        aimCircle1 = this.gameObject.transform.GetChild(2).gameObject;
        aimCircle2 = this.gameObject.transform.GetChild(3).gameObject;

        aimlockMaterial = aimlock.GetComponent<MeshRenderer>().material;
        explosion.SetActive(false);
        duration = explosion.GetComponent<ParticleSystem>().main.duration - 1;
        explosion.GetComponent<ParticleSystem>().Stop();
        StartCoroutine(CountdownToAttack());
        StartCoroutine(Shaking());
        StartCoroutine(AimlockController());
    }

    private void Attack()
    {
        gameObject.layer = 0;
        Destroy(aimlock);
        StopCoroutine(AimlockController());
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
                Destroy(gameObject);
            }
            Countdown();
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator CountdownToAttack()
    {
        while (time <= TIME_TO_ATTACK)
        {
            if (time == TIME_TO_ATTACK)
            {
                Attack();
                StopCoroutine(CountdownToAttack());
            }
            TimeCount();
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator AimlockController()
    {
        Debug.Log("Aimlock");
        Vector3 scaleChange = new Vector3(0.003f, 0.003f, 0.003f);

        bool wasYellow = false;

        while (time <= TIME_TO_ATTACK)
        {
            if (aimlock.transform.localScale.x >= 10)
            {
                aimlock.transform.localScale -= scaleChange * 300;
            }

            if (aimCircle1 != null)
            {
                aimCircle1.transform.localScale -= (scaleChange * 500);
                if (aimCircle1.transform.localScale.x <= 0)
                {
                    Destroy(aimCircle1);
                }
            }

            if (aimCircle2 != null)
            {
                aimCircle2.transform.localScale -= (scaleChange * 900);
                if (aimCircle2.transform.localScale.x <= 0)
                {
                    Destroy(aimCircle2);
                }
            }

            aimlock.transform.Rotate(0f, 75 * Time.fixedDeltaTime, 0f);

            if (wasYellow)
            {
                aimlockMaterial.color = new Color(aimlockMaterial.color.r, aimlockMaterial.color.g - Time.fixedDeltaTime * 0.4f, aimlockMaterial.color.b);
            }
            else
            {
                aimlockMaterial.color = new Color(aimlockMaterial.color.r + Time.fixedDeltaTime * 0.35f, aimlockMaterial.color.g, aimlockMaterial.color.b);
                if (aimlockMaterial.color.r >= 1 && aimlockMaterial.color.g >= 1)
                {
                    wasYellow = true;
                }
            }

            yield return null;
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
        time += 1;
    }
}
