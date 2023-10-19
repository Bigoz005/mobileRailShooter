using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject explosion;
    private float duration;
    private float time = 0;
    private const int TIME_TO_ATTACK = 10;

    Vector3 startingPos;

    void Awake()
    {
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;
        startingPos.z = transform.position.z;
    }

    private void Start()
    {
        explosion = this.gameObject.transform.GetChild(0).gameObject;
        explosion.SetActive(false);
        duration = explosion.GetComponent<ParticleSystem>().main.duration - 1;
        explosion.GetComponent<ParticleSystem>().Stop();
        StartCoroutine(CountdownToAttack());
    }

    private void Attack()
    {
        gameObject.layer = 0;
        explosion.SetActive(true);
        explosion.GetComponent<ParticleSystem>().Play();
        Camera.main.GetComponentInChildren<Player>().GetHit();
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

    private void Countdown()
    {
        duration -= 1;
    }

    private void TimeCount()
    {
        time += 1;
    }
}
