using System.Collections;
using UnityEngine;
using EZCameraShake;
using System.Threading.Tasks;

public class EnemyHard : Enemy
{
    private Camera camMain;
    private void OnEnable()
    {
        camMain = Camera.main;
        audioSource = GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>();
        if (PlayerPrefs.GetInt("Difficulty", 0) == 2)
        {
            GameObject.FindGameObjectWithTag("SoundPlayer").GetComponent<AudioSource>().pitch = 0.5f;
            audioSource.pitch = 0.75f;
        }

        index = Random.Range(1, 4);
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;
        startingPos.z = transform.position.z;
        explosion = this.gameObject.transform.GetChild(0).gameObject;
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        transform.LookAt(camMain.transform);
        time = 0;

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

        zoomController.SetVariables(camMain, objectToWatch, this.gameObject, camMain.transform.GetChild(1).GetComponent<Camera>(), specialIndex);

        StartCoroutine(zoomController.ZoomOnEnemy());
        StartCoroutine(zoomController.Move());
        StartCoroutine(CountdownToAttack());
        StartCoroutine(Shaking());
    }

    private IEnumerator CountdownToAttack()
    {
        while (time <= TIME_TO_ATTACK)
        {
            if (time == TIME_TO_ATTACK - 1f && enabled)
            {
                audioSource.clip = explosionClip;
                audioSource.Play();
            }

            if (time >= TIME_TO_ATTACK - 0.5f)
            {
                Attack();
                yield return null;
            }

            TimeCount();
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }

    private void Attack()
    {
        if (enabled)
        {
            gameObject.layer = 0;

            camMain.GetComponentInChildren<Player>().GetHit();
            if (camMain.GetComponentInChildren<Player>().GetHealth() != 0)
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
            yield return new WaitUntil(() => camMain.gameObject.GetComponent<SystemPreferences>().IsPaused == false);
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
