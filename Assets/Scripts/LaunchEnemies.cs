using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchEnemies : MonoBehaviour
{
    private int previousIndex;
    private int index;
    private float duration = 2f;
    private bool isCounting = false;
    private GameObject activeEnemy;
    public Coroutine activeCoroutineActivated;
    public Coroutine activeCoroutineCountdown;

    private void Start()
    {
        previousIndex = -1;
        activeCoroutineActivated = StartCoroutine(CountdownToActivate());
        if(PlayerPrefs.GetInt("Difficulty", 0) == 2)
        {
            GameObject.FindGameObjectWithTag("SoundPlayer").GetComponent<AudioSource>().pitch = 0.5f;
            GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>().pitch = 0.75f;
        }
        else
        {
            GameObject.FindGameObjectWithTag("SoundPlayer").GetComponent<AudioSource>().pitch = 1.5f;
            GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>().pitch = 1f;
        }
    }

    public IEnumerator ActivateRandomEnemy()
    {
        index = Random.Range(0, 24);

        int specialGroupIndex = Random.Range(0, 2);
        int specialIndex = Random.Range(0, 3);

        while (previousIndex == index)
        {
            index = Random.Range(0, 24);
        }
        previousIndex = index;
        duration = 2;

        if (isCounting)
        {
            StopCoroutine(activeCoroutineCountdown);
        }

        activeEnemy = gameObject.transform.GetChild(index).gameObject;
        activeEnemy.tag = "Enemy";
        activeEnemy.SetActive(true);
        activeEnemy.GetComponent<MeshRenderer>().enabled = true;
        

        if (!activeEnemy.transform.name.Contains("Hard"))
        {
            activeEnemy.transform.GetChild(1).gameObject.SetActive(true);
            activeEnemy.transform.GetChild(2).gameObject.SetActive(true);
            activeEnemy.transform.GetChild(3).gameObject.SetActive(true);
            activeEnemy.GetComponent<Enemy>().enabled = true;
        }
        else
        {
            activeEnemy.GetComponent<EnemyHard>().enabled = true;
        }

        while (activeEnemy.activeInHierarchy)
        {
            //do nothing while active then start countdown to activate
            yield return null;
        }

        activeCoroutineCountdown = StartCoroutine(CountdownToActivate());
        yield return null;
    }

    public IEnumerator CountdownToActivate()
    {
        if (activeEnemy)
        {
            if (activeEnemy.transform.name.Contains("Hard"))
            {
                duration = 0.5f;
            }
            else
            if (activeEnemy.transform.name.Contains("Medium"))
            {
                duration = 1;
            }
        }

        isCounting = true;
        if (activeCoroutineActivated != null)
        {
            StopCoroutine(activeCoroutineActivated);
        }
        while (duration > 0)
        {
            Countdown();
            yield return new WaitForSeconds(1);
        }

        if (duration == 0)
        {
            isCounting = false;
            StartCoroutine(ActivateRandomEnemy());
        }

        yield return null;
    }

    private void Countdown()
    {
        duration -= 0.5f;
    }
}
