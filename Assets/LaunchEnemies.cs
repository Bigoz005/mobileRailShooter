using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchEnemies : MonoBehaviour
{
    private int previousIndex;
    private int index;
    private int duration = 2;
    private bool isCounting = false;
    private GameObject activeEnemy;

    private void Start()
    {
        previousIndex = -1;
        StartCoroutine(ActivateRandomEnemy());
    }

    private IEnumerator ActivateRandomEnemy()
    {
        index = Random.Range(0, 24);
        while (previousIndex == index)
        {
            index = Random.Range(0, 24);
        }
        previousIndex = index;
        duration = 2;

        if (isCounting)
        {
            StopCoroutine(CountdownToActivate());
        }

        activeEnemy = gameObject.transform.GetChild(index).gameObject;
        activeEnemy.SetActive(true);

        while (activeEnemy.activeInHierarchy)
        {
            yield return null;
        }

        StartCoroutine(CountdownToActivate());
    }

    private IEnumerator CountdownToActivate()
    {
        isCounting = true;
        StopCoroutine(ActivateRandomEnemy());
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
        duration -= 1;
    }
}
