using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zooming : MonoBehaviour
{
    private float zoom;
    private float zoomMultiplier = 20.0f;
    private float minFieldOfView = 35.0f;
    private float maxFieldOfView = 55.0f;
    private float velocity = 0f;
    private float smoothTime = 0.25f;
    private GameObject enemy;
    private Camera cam;
    private float time;
    private float time_to_attack;
    private GameObject gameobjectToWatch;
    private Camera circleCam = null;

    private bool runningZoom = false;
    private bool runningMove = false;

    private static Zooming zoomingManagerInstance;

    void Awake()
    {
        if (zoomingManagerInstance == null)
        {
            zoomingManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (zoomingManagerInstance != this)
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator ZoomOnEnemy()
    {
        minFieldOfView = 35.0f;
        if (enemy.GetComponent<Enemy>())
        {
            time = enemy.GetComponent<Enemy>()._Time;
            time_to_attack = enemy.GetComponent<Enemy>()._TIME_TO_ATTACK;
        }
        else
        {
            time = enemy.GetComponent<EnemyHard>()._Time;
            time_to_attack = enemy.GetComponent<EnemyHard>()._TIME_TO_ATTACK;
        }

        Vector3 dir = enemy.transform.position - cam.transform.position;
        RaycastHit hit;

        Debug.DrawRay(cam.transform.position, dir, Color.yellow, 1000);

        if (Physics.Raycast(cam.transform.position, dir, out hit, 10000))
        {
            minFieldOfView = minFieldOfView - Mathf.Log(hit.distance * 3.0f, 1.2f);
            if (minFieldOfView < 6)
            {
                minFieldOfView = 6;
            }
        }
        else
        {
            minFieldOfView = 10;
        }

        while (true)
        {

            zoom -= 5 * zoomMultiplier;
            zoom = Mathf.Clamp(zoom, minFieldOfView, maxFieldOfView);

            cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, zoom, ref velocity, smoothTime);
            circleCam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, zoom, ref velocity, smoothTime);
            if (Mathf.Abs(cam.fieldOfView - minFieldOfView) < 0.1 || runningZoom)
            {
                break;
            }

            yield return null;
        }
        yield return null;
    }

    public IEnumerator Move()
    {
        GameObject targetGameObject;

        float rand = Random.Range(0, 2);
        int randInside = Random.Range(0, 4);

        if (rand == 1)
        {
            if (enemy.transform.GetChild(5).CompareTag("RightBox"))
            {
                targetGameObject = enemy.transform.GetChild(5).GetChild(randInside).gameObject;
            }
            else
            {
                targetGameObject = enemy.transform.GetChild(2).GetChild(randInside).gameObject;
            }

        }
        else
        {
            if (enemy.transform.GetChild(4).CompareTag("LeftBox"))
            {
                targetGameObject = enemy.transform.GetChild(4).GetChild(randInside).gameObject;
            }
            else
            {
                targetGameObject = enemy.transform.GetChild(1).GetChild(randInside).gameObject;
            }
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetGameObject.transform.position - cam.transform.position);

        while (true)
        {
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, 4.5f * Time.deltaTime);
            if ((Mathf.Abs(targetRotation.eulerAngles.x - cam.transform.rotation.eulerAngles.x) < 1 && Mathf.Abs(targetRotation.eulerAngles.y - cam.transform.rotation.eulerAngles.y) < 1 && Mathf.Abs(targetRotation.eulerAngles.z - cam.transform.rotation.eulerAngles.z) < 1) || runningMove)
            {
                break;
            }
            yield return null;
        }
        yield return null;
    }

    public IEnumerator MoveBack()
    {
        Quaternion targetRotation = Quaternion.LookRotation(gameobjectToWatch.transform.position - cam.transform.position);

        while (true)
        {
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, 4.5f * Time.deltaTime);
            if ((Mathf.Abs(targetRotation.eulerAngles.x - cam.transform.rotation.eulerAngles.x) < 1 && Mathf.Abs(targetRotation.eulerAngles.y - cam.transform.rotation.eulerAngles.y) < 1 && Mathf.Abs(targetRotation.eulerAngles.z - cam.transform.rotation.eulerAngles.z) < 1))
            {
                break;
            }

            yield return null;
        }
        yield return null;
    }

    public IEnumerator ZoomOutEnemy()
    {
        while (true)
        {
            zoom += 30 * zoomMultiplier;
            zoom = Mathf.Clamp(zoom, minFieldOfView, maxFieldOfView);

            cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, zoom, ref velocity, smoothTime);
            circleCam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, zoom, ref velocity, smoothTime);

            if (Mathf.Abs(cam.fieldOfView - maxFieldOfView) < 1)
            {
                break;
            }

            yield return null;
        }
        yield return null;
    }

    public GameObject GetEnemy()
    {
        return enemy;
    }

    public void SetEnemy(GameObject enemyToAssign)
    {
        enemy = enemyToAssign;
    }

    public void SetCircleCam(Camera camToAssign)
    {
        circleCam = camToAssign;
    }
    public void SetVariables(Camera camera, GameObject gameObjectToWatch, GameObject enemy, Camera camToAssign)
    {
        SetEnemy(enemy);
        SetCircleCam(camToAssign);
        cam = camera;
        zoom = cam.fieldOfView;
        gameobjectToWatch = gameObjectToWatch;
    }
}
