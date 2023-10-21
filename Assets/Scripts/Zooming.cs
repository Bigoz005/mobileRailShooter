using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zooming : MonoBehaviour
{
    private float zoom;
    private float zoomMultiplier = 20.0f;
    private float move;
    private float moveMultiplier = 10.0f;
    private float minFieldOfView = 35.0f;
    private float maxFieldOfView = 65.0f;
    private float velocity = 0f;
    private float smoothTime = 0.25f;
    private GameObject enemy;
    private Camera cam;
    private bool isMoved = false;
    private float time;
    private float time_to_attack;
    private Quaternion mainRotation;
    private Quaternion targetRotation;
    private Quaternion startRotation;

    void Start()
    {
        mainRotation = transform.rotation;
        cam = Camera.main;
        zoom = cam.fieldOfView;
    }

    public IEnumerator ZoomOnEnemy()
    {
        time = enemy.GetComponent<Enemy>()._Time;
        time_to_attack = enemy.GetComponent<Enemy>()._TIME_TO_ATTACK;
        while (time <= time_to_attack)
        {
            zoom -= 10 * zoomMultiplier;
            zoom = Mathf.Clamp(zoom, minFieldOfView, maxFieldOfView);

            yield return null;
        }
    }

    public IEnumerator ZoomOutEnemy()
    {
        while (time <= time_to_attack)
        {
            zoom += 10 * zoomMultiplier;
            zoom = Mathf.Clamp(zoom, minFieldOfView, maxFieldOfView);

            cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, zoom, ref velocity, smoothTime);
            yield return null;
        }
    }

    public IEnumerator Move()
    {
        GameObject targetGameObject;

        /*Sprawdzanie czy krasnal jest z lewej czy z prawej - nie dziala poprawnie*/

        /*Vector3 playerVector = Camera.main.transform.forward;
        Ray ray = new Ray(Camera.main.transform.position, enemy.transform.position);
        Vector3 enemyVector = ray.direction;
        float difference = Vector3.Angle(playerVector, enemyVector);*/

        float rand = Random.Range(-1f, 1f);

        if (rand > 0)
        {
            targetGameObject = enemy.transform.Find("Right").gameObject;
        }
        else
        {
            targetGameObject = enemy.transform.Find("Left").gameObject;
        }

        while (time <= time_to_attack)
        {
            var targetRotation = Quaternion.LookRotation(targetGameObject.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.8f * Time.deltaTime);
            yield return null;
        }
    }

    /* public IEnumerator MoveBack()
     {
         Vector3 playerVector = transform.forward;
         Ray ray = new Ray(transform.position, enemy.transform.position);
         Vector3 enemyVector = ray.direction;
         float difference = Vector3.Angle(playerVector, enemyVector);

         while (time <= enemy.GetComponent<Enemy>()._TIME_TO_ATTACK)
         {
             //check if zoomed out -> stop coroutine of zoomingOut then change transform.LookAt to default 
             transform.rotation = Quaternion.Slerp(targetRotation, mainRotation, 5 * Time.fixedTime);
             yield return null;
         }
     }*/

    /*public IEnumerator Move()
    {
        Vector3 playerVector = transform.forward;
        Ray ray = new Ray(transform.position, enemy.transform.position);
        Vector3 enemyVector = ray.direction;
        float difference = Vector3.Angle(playerVector, enemyVector);
        
        while (time <= enemy.GetComponent<Enemy>()._TIME_TO_ATTACK)
        {
            //check if zoomed on -> stop coroutine of zoomingOnEnemy
            CacheStartOnceBefore();
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, 5 * Time.fixedTime);

            yield return null;
        }
    }
    

    private void CacheStartOnceBefore()
    {
        if (isMoved) return;

        targetRotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
        startRotation = transform.rotation;
        isMoved = true;
    }*/


    public GameObject GetEnemy()
    {
        return enemy;
    }

    public void SetEnemy(GameObject enemyToAssign)
    {
        enemy = enemyToAssign;
    }
}
