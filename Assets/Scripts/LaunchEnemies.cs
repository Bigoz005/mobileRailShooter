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
        activeEnemy.SetActive(true);
        activeEnemy.GetComponent<MeshRenderer>().enabled = true;

        if (!activeEnemy.transform.name.Contains("Hard"))
        {
            activeEnemy.transform.GetChild(1).gameObject.SetActive(true);
            activeEnemy.transform.GetChild(2).gameObject.SetActive(true);
            activeEnemy.transform.GetChild(3).gameObject.SetActive(true);
        }

        while (activeEnemy.activeInHierarchy)
        {
            yield return null;
        }

        activeCoroutineCountdown = StartCoroutine(CountdownToActivate());
    }

    public IEnumerator CountdownToActivate()
    {
        if (activeEnemy)
        {
            if (activeEnemy.transform.name.Contains("Hard"))
            {
                duration = 0.5f;
                Debug.Log(duration);
            }
            else
            if (activeEnemy.transform.name.Contains("Medium"))
            {
                duration = 1;
                Debug.Log(duration);
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
    }

    private void Countdown()
    {
        duration -= 0.5f;
    }
}
