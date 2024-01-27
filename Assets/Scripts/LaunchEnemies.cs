using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchEnemies : MonoBehaviour
{
    private int previousIndex;
    private int index;
    private float duration = 2f;
    private float timeToActivate = 4f;
    private bool isCounting = false;
    private int points;
    private GameObject activeEnemy;
    public Coroutine activeCoroutineActivated;
    public Coroutine activeCoroutineCountdown;

    private void Start()
    {
        previousIndex = -1;
        points = Camera.main.GetComponent<Player>().GetPoints();
        activeCoroutineActivated = StartCoroutine(CountdownToActivate());
        if (PlayerPrefs.GetInt("Difficulty", 0) == 2)
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
        activeEnemy.layer = LayerMask.NameToLayer("RayCast");
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
            yield return null;
        }

        activeCoroutineCountdown = StartCoroutine(CountdownToActivate());
        yield return null;
    }

    public IEnumerator CountdownToActivate()
    {
        duration = 0.7f;
        if (activeEnemy)
        {
            if (activeEnemy.transform.name.Contains("Hard"))
            {
                duration = 0.4f;
            }
            else
            if (activeEnemy.transform.name.Contains("Medium"))
            {
                duration = 0.55f;
            }
        }

        int score = Camera.main.GetComponent<Player>().GetScore();
        float multiplier = 1f;
        switch (score)
        {
            case > 1250 and < 2500:
                multiplier = 0.9f;
                break;
            case > 2500 and < 5000:
                multiplier = 0.8f;
                break;
            case > 5000 and < 10000:
                multiplier = 0.75f;
                break;
            case > 10000 and < 20000:
                multiplier = 0.7f;
                break;
            case > 20000 and < 30000:
                multiplier = 0.65f;
                break;
            case > 30000 and < 41000:
                multiplier = 0.6f;
                break;
            case > 41000 and < 53000:
                multiplier = 0.5f;
                break;
            case > 53000 and < 67000:
                multiplier = 0.47f;
                break;
            case > 67000 and < 85000:
                multiplier = 0.44f;
                break;
            case > 85000 and < 128000:
                multiplier = 0.4f;
                break;
            case > 128000 and < 150000:
                multiplier = 0.35f;
                break;
            case > 150000 and < 200000:
                multiplier = 0.3f;
                break;
            case > 200000 and < 250000:
                multiplier = 0.2f;
                break;
            case > 250000 and < 350000:
                multiplier = 0.15f;
                break;
            case > 350000:
                multiplier = 0.1f;
                break;
        }
        Camera.main.GetComponent<Player>().SetPoints(points * 2 * (2 - multiplier));

        isCounting = true;
        if (activeCoroutineActivated != null)
        {
            StopCoroutine(activeCoroutineActivated);
        }
        while (timeToActivate > 0)
        {
            Countdown();
            yield return new WaitForSeconds(multiplier * duration);
        }

        if (timeToActivate <= 0)
        {
            isCounting = false;
            StartCoroutine(ActivateRandomEnemy());
        }

        yield return null;
    }

    private void Countdown()
    {
        timeToActivate -= 1f;
    }
}
