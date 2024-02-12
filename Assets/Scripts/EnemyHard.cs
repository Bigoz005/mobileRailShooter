using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using System.Threading.Tasks;

public class EnemyHard : Enemy
{

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("Difficulty", 0) == 2)
        {
            GameObject.FindGameObjectWithTag("SoundPlayer").GetComponent<AudioSource>().pitch = 0.5f;
            GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>().pitch = 0.75f;
        }

        index = Random.Range(1, 4);
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;
        startingPos.z = transform.position.z;
        explosion = this.gameObject.transform.GetChild(0).gameObject;
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        transform.LookAt(Camera.main.transform);
        zoomController = GameObject.FindGameObjectWithTag("ZoomController").GetComponent<Zooming>();
        GameObject objectToWatch = GameObject.FindGameObjectWithTag("MainGameObjectToWatch");
        time = 0;

        audioSource = GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>();

        explosion.SetActive(false);
        duration = explosion.GetComponent<ParticleSystem>().main.duration - 1;

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

        zoomController.SetVariables(Camera.main, objectToWatch, this.gameObject, Camera.main.transform.GetChild(1).GetComponent<Camera>());

        StartCoroutine(zoomController.ZoomOnEnemy());
        StartCoroutine(zoomController.Move());
        CountdownToAttack();
        StartCoroutine(Shaking());
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

                if (time == TIME_TO_ATTACK - 1f && enabled)
                {
                    audioSource.clip = explosionClip;
                    audioSource.Play();
                }

                if (time == TIME_TO_ATTACK - 0.5f)
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
    private void Attack()
    {
        if (enabled)
        {
            gameObject.layer = 0;

            Camera.main.GetComponentInChildren<Player>().GetHit();
            if (Camera.main.GetComponentInChildren<Player>().GetHealth() != 0)
            {
                CameraShaker.Instance.ShakeOnce(3f, 3f, 0.34f, 0.34f);
            }
            explosion.SetActive(true);
        }
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        StartCoroutine(zoomController.MoveBack());
        StartCoroutine(zoomController.ZoomOutEnemy());
        StartCoroutine(CountdownToExtinction());
    }

    public new IEnumerator CountdownToExtinction()
    {
        ResetSpecialItem();

        while (duration >= 0)
        {
            if (duration == 0)
            {
                gameObject.SetActive(false);
                gameObject.transform.GetChild(3).gameObject.SetActive(false);
                gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            Countdown();
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }

    private IEnumerator Shaking()
    {

        Vector3 startPostition = transform.position;

        while (time <= TIME_TO_ATTACK && enabled)
        {
            float strength = curve.Evaluate(time * 4 / TIME_TO_ATTACK);
            transform.position = startPostition + Random.insideUnitSphere * strength;
            yield return new WaitUntil(() => Camera.main.gameObject.GetComponent<SystemPreferences>().IsPaused == false);
        }

        transform.position = startPostition;
        yield return null;
    }

    public new void StopAllGnomeCoroutines()
    {
        StopCoroutine(Shaking());
        StopCoroutine(CountdownToExtinction());
    }
}
