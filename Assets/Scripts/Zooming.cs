using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zooming : MonoBehaviour
{
    private float zoom;
    private float zoomMultiplier = 20.0f;
    private float minFieldOfView = 35.0f;
    private float maxFieldOfView = 65.0f;
    private float velocity = 0f;
    private float smoothTime = 0.25f;
    private GameObject enemy;
    private Camera cam;
    private float time;
    private float time_to_attack;
    private GameObject gameobjectToWatch;

    void Start()
    {
        cam = Camera.main;
        zoom = cam.fieldOfView;
        gameobjectToWatch = GameObject.FindGameObjectWithTag("MainGameObjectToWatch");
    }

    public IEnumerator ZoomOnEnemy()
    {
        minFieldOfView = 35.0f;
        if (enemy.GetComponent<Enemy>()) { 
            time = enemy.GetComponent<Enemy>()._Time;
            time_to_attack = enemy.GetComponent<Enemy>()._TIME_TO_ATTACK;
        }
        else
        {
            time = enemy.GetComponent<EnemyHard>()._Time;
            time_to_attack = enemy.GetComponent<EnemyHard>()._TIME_TO_ATTACK;
        }

        Vector3 dir = enemy.transform.position - Camera.main.transform.position;
        Ray ray = new Ray(Camera.main.transform.position, dir);
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, dir, out hit, 10000))
        {
            Debug.Log("distance: " + hit.distance);
            Debug.Log("Log(distance): " + Mathf.Log(hit.distance*1.7f, 1.2f));
            minFieldOfView = minFieldOfView - Mathf.Log(hit.distance*1.7f, 1.2f);
            if(minFieldOfView < 6)
            {
                minFieldOfView = 6;
            }
            
        }
        Debug.Log("minField: " + minFieldOfView);

        while (time <= time_to_attack)
        {
            zoom -= 5 * zoomMultiplier;
            zoom = Mathf.Clamp(zoom, minFieldOfView, maxFieldOfView);

            cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, zoom, ref velocity, smoothTime);
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
            Quaternion targetRotation = Quaternion.LookRotation(targetGameObject.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1.5f * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator MoveBack()
     {
        /*Vector3 playerVector = transform.forward;
        Ray ray = new Ray(transform.position, enemy.transform.position);
        Vector3 enemyVector = ray.direction;
        float difference = Vector3.Angle(playerVector, enemyVector);*/

        bool rotated = false;
         while (time <= time_to_attack)
         {
            //check if zoomed out -> stop coroutine of zoomingOut then change transform.LookAt to default 
            Quaternion targetRotation = Quaternion.LookRotation(gameobjectToWatch.transform.position - transform.position);
            if(transform.rotation != targetRotation && !rotated) { 
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1.5f * Time.deltaTime);
            }
            else
            {
                rotated = true;
            }

            yield return null;
        }
     }

    public GameObject GetEnemy()
    {
        return enemy;
    }

    public void SetEnemy(GameObject enemyToAssign)
    {
        enemy = enemyToAssign;
    }
}
