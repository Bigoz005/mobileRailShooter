using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchEnemies : MonoBehaviour
{
    private int previousIndex;
    private int index;
    private int duration = 3;
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
        while (previousIndex == index)
        {
            index = Random.Range(0, 24);
        }
        previousIndex = index;
        duration = 3;

        if (isCounting)
        {
            StopCoroutine(activeCoroutineCountdown);
        }

        activeEnemy = gameObject.transform.GetChild(index).gameObject;
        activeEnemy.SetActive(true);
        activeEnemy.GetComponent<MeshRenderer>().enabled = true;
        activeEnemy.transform.GetChild(1).gameObject.SetActive(true);
        activeEnemy.transform.GetChild(2).gameObject.SetActive(true);
        activeEnemy.transform.GetChild(3).gameObject.SetActive(true);

        while (activeEnemy.activeInHierarchy)
        {
            yield return null;
        }

        activeCoroutineCountdown = StartCoroutine(CountdownToActivate());
    }

    public IEnumerator CountdownToActivate()
    {
        isCounting = true;
        if (activeCoroutineActivated != null) { 
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
        duration -= 1;
    }
}
