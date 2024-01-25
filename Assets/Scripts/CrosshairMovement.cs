using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CrosshairMovement : MonoBehaviour
{

    [SerializeField]
    GameObject joystick;
    [SerializeField]
    GameObject self;
    [SerializeField]
    GameObject reloadCircle;
    [SerializeField]
    GameObject shootButton;
    [SerializeField]
    GameObject gameplayCanvas;
    [SerializeField]
    GameObject gameOverCanvas;

    public GameObject LasersObject;
    public Camera circleCamera;

    private Camera cam;
    private Vector3 previousPosition = new Vector3(0, 0, 0);
    private AudioSource audioSource;
    private AudioSource enemyAudioSource;
    private MusicManager musicManager;
    string[] layerNames = { "RayCast", "Specials" };
    private float previousTime;
    private float actualTime;
    private ParticleSystem particle = null;

    private AudioClip shootClip;
    private AudioClip bonusClip;
    private AudioClip healthClip;
    private AudioClip powerUpClip;
    private AudioClip gnomeClip;

    private float duration = 17.0f;
    private float reloadDuration = 0.5f;
    private int points;
    private bool touchControlEnabled;
    private int i = 1;
    private int previousIndex;
    private int index;
    private int powerupMultiplier = 1;
    private bool isReloding = false;

    public Light directionalLight;
    private Color lightColor;
    private Color[] colors = new Color[7];

    private void Start()
    {
        cam = Camera.main;
        audioSource = GameObject.FindGameObjectWithTag("SoundPlayer").GetComponent<AudioSource>();
        enemyAudioSource = GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>();
        musicManager = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicManager>();
        shootClip = transform.GetComponent<Shoot>().shootClip;
        bonusClip = transform.GetComponent<Shoot>().bonusClip;
        healthClip = transform.GetComponent<Shoot>().healthClip;
        powerUpClip = transform.GetComponent<Shoot>().powerUpClip;
        gnomeClip = transform.GetComponent<Shoot>().gnomeClip;
        points = Camera.main.GetComponent<Player>().GetPoints();
        points = points * (PlayerPrefs.GetInt("Difficulty", 0) + 1);
        previousTime = Time.deltaTime;
        actualTime = previousTime;

        colors[0] = Color.blue;
        colors[1] = Color.cyan;
        colors[2] = Color.green;
        colors[3] = Color.magenta;
        colors[4] = Color.red;
        colors[5] = Color.yellow;
        colors[6] = directionalLight.color;

        if (PlayerPrefs.GetInt("Controls") == 1)
        {
            touchControlEnabled = true;
            joystick.transform.parent.parent.gameObject.SetActive(false);
            Camera.main.gameObject.GetComponent<Player>().controlsScoreDividor = 5;
            shootButton.SetActive(false);
            self.GetComponent<Image>().enabled = false;
        }
        else
        {
            touchControlEnabled = false;
            Camera.main.gameObject.GetComponent<Player>().controlsScoreDividor = 1;
            joystick.transform.parent.gameObject.SetActive(true);
            shootButton.SetActive(true);
            self.GetComponent<Image>().enabled = true;
        }
    }

    void Update()
    {
        actualTime = actualTime + Time.deltaTime;
        if (!touchControlEnabled)
        {
            if (previousPosition.x != joystick.transform.localPosition.x || previousPosition.y != joystick.transform.localPosition.y)
            {
                previousPosition = joystick.transform.localPosition;
                self.transform.localPosition = new Vector3(previousPosition.x * 6.75f, previousPosition.y * 3.1f, previousPosition.z);
                if (!isReloding)
                {
                    reloadCircle.transform.localPosition = self.transform.localPosition;
                }
            }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                    if (!isReloding)
                    {
                        Vector2 anchoredPosition;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(reloadCircle.transform.parent.transform.parent.GetComponent<RectTransform>(), Input.GetTouch(0).position, circleCamera, out anchoredPosition);
                        reloadCircle.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
                    }

                    checkShoot(ray);

                    if (Physics.Raycast(ray))
                        Debug.DrawLine(cam.transform.position, ray.direction * 10000000000, Color.red, 20);


                }

            }
        }
    }

    private void checkShoot(Ray ray)
    {
        RaycastHit hit;
        reloadDuration = 2.0f;

        if (!musicManager.powerUpOn)
        {
            points = Camera.main.GetComponent<Player>().GetPoints();
            points = points * (PlayerPrefs.GetInt("Difficulty", 0) + 1);
        }

        if (!gameOverCanvas.transform.GetChild(3).GetComponent<MyButton>().buttonPressed && !gameplayCanvas.transform.GetChild(6).GetComponent<MyButton>().buttonPressed && gameplayCanvas.activeSelf && !reloadCircle.GetComponent<Image>().enabled && (actualTime - previousTime) > 0.6f)
        {
            previousTime = actualTime;
            audioSource.clip = shootClip;
            int tempPoints = 0;
            if (Physics.SphereCast(ray, 0.1f, out hit, 10000000000, LayerMask.GetMask(layerNames)))
            {
                switch (hit.collider.tag)
                {
                    case "Enemy":
                        hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                        enemyAudioSource.Stop();
                        enemyAudioSource.clip = gnomeClip;

                        if (musicManager.powerUpOn)
                        {
                            tempPoints = powerupMultiplier * (points / 10 / Camera.main.gameObject.GetComponent<Player>().controlsScoreDividor);
                        }
                        else
                        {
                            tempPoints = points / 10 / Camera.main.gameObject.GetComponent<Player>().controlsScoreDividor;
                        }

                        Camera.main.gameObject.GetComponent<Player>().AddScore(tempPoints);

                        if (hit.collider.name.Contains("Hard"))
                        {
                            hit.collider.gameObject.GetComponent<EnemyHard>().StopAllGnomeCoroutines();
                            hit.collider.gameObject.GetComponent<EnemyHard>().enabled = false;
                            hit.collider.gameObject.transform.GetChild(3).gameObject.SetActive(true);
                            hit.collider.gameObject.transform.GetChild(4).GetComponent<TextMeshPro>().SetText("+ " + tempPoints);
                            hit.collider.gameObject.transform.GetChild(4).gameObject.SetActive(true);
                            StartCoroutine(pointsTextVisibility(hit.collider.gameObject.transform.GetChild(4).gameObject));
                            hit.collider.gameObject.tag = "Untagged";
                        }
                        else
                        {
                            hit.collider.gameObject.GetComponent<Enemy>().ResetAimlockAndCircles();
                            hit.collider.gameObject.GetComponent<Enemy>().StopAllGnomeCoroutines();
                            hit.collider.gameObject.GetComponent<Enemy>().enabled = false;
                            hit.collider.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                            hit.collider.gameObject.transform.GetChild(2).gameObject.SetActive(false);
                            hit.collider.gameObject.transform.GetChild(3).gameObject.SetActive(false);
                            hit.collider.gameObject.transform.GetChild(6).gameObject.SetActive(true);
                            hit.collider.gameObject.transform.GetChild(7).GetComponent<TextMeshPro>().SetText("+ " + tempPoints);
                            hit.collider.gameObject.transform.GetChild(7).gameObject.SetActive(true);
                            StartCoroutine(pointsTextVisibility(hit.collider.gameObject.transform.GetChild(7).gameObject));
                            hit.collider.gameObject.tag = "Untagged";
                        }
                        enemyAudioSource.Play();

                        break;
                    case "ScorePowerUp":
                        particle = hit.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                        hit.collider.gameObject.transform.GetChild(1).GetComponent<TextMeshPro>().SetText("+ " + points / Camera.main.gameObject.GetComponent<Player>().controlsScoreDividor);
                        hit.collider.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                        StartCoroutine(pointsTextVisibility(hit.collider.gameObject.transform.GetChild(1).gameObject));
                        hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                        if (particle.isPlaying)
                        {
                            particle.Stop();
                        }
                        particle.Play();

                        audioSource.clip = bonusClip;
                        Camera.main.gameObject.GetComponent<Player>().AddScore(points / Camera.main.gameObject.GetComponent<Player>().controlsScoreDividor);
                        break;

                    case "BonusHealth":
                        hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                        particle = hit.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                        if (particle.isPlaying)
                        {
                            particle.Stop();
                        }
                        particle.Play();

                        audioSource.clip = healthClip;
                        int i = Camera.main.gameObject.GetComponent<Player>().AddHealth(points / 2 / Camera.main.gameObject.GetComponent<Player>().controlsScoreDividor, hit.collider.gameObject.transform.GetChild(1).gameObject);
                        if (i == 1)
                        {
                            hit.collider.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                            StartCoroutine(pointsTextVisibility(hit.collider.gameObject.transform.GetChild(1).gameObject));
                        }
                        break;
                    case "PowerUp":
                        hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                        particle = hit.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                        if (particle.isPlaying)
                        {
                            particle.Stop();
                        }
                        particle.Play();

                        musicManager.playPowerUpMusic();
                        audioSource.clip = powerUpClip;
                        LasersObject.SetActive(true);
                        if (musicManager.powerUpOn)
                        {
                            StopCoroutine(powerUpDuration());
                            StopCoroutine(ChangeLightColor());
                        }
                        StartCoroutine(ChangeLightColor());
                        StartCoroutine(powerUpDuration());
                        break;
                    case "Target":

                        if (musicManager.powerUpOn)
                        {
                            tempPoints = powerupMultiplier * points / 10 / Camera.main.gameObject.GetComponent<Player>().controlsScoreDividor;
                        }
                        else
                        {
                            tempPoints = powerupMultiplier * points / 10 / Camera.main.gameObject.GetComponent<Player>().controlsScoreDividor / 2;
                        }
                        Camera.main.gameObject.GetComponent<Player>().AddScore(tempPoints);
                        hit.collider.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().SetText("+ " + tempPoints);
                        hit.collider.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        StartCoroutine(pointsTextVisibility(hit.collider.gameObject.transform.GetChild(0).gameObject));
                        break;
                }
            }
            self.GetComponent<Image>().enabled = false;
            StartCoroutine(reload());
            if (!(enemyAudioSource.clip == gnomeClip && enemyAudioSource.isPlaying))
            {
                audioSource.Play();
            }
        }
    }

    private IEnumerator pointsTextVisibility(GameObject gameObject)
    {
        i = 1;
        while (i > 0)
        {
            CountdownVisibility();
            yield return new WaitForSeconds(1);
        }
        gameObject.SetActive(false);
        yield return null;
    }
    private IEnumerator reload()
    {
        Image image = reloadCircle.GetComponent<Image>();
        image.enabled = true;
        isReloding = true;
        while (image.fillAmount > 0)
        {
            ReloadCountdown();
            image.fillAmount -= 0.03f;
            yield return new WaitForSeconds(0.01f);
        }
        if (image.fillAmount <= 0.1)
        {
            image.fillAmount = 1;
        }
        if (PlayerPrefs.GetInt("Controls") != 1)
        {
            self.GetComponent<Image>().enabled = true;
        }
        image.enabled = false;
        isReloding = false;
        yield return null;
    }

    private IEnumerator powerUpDuration()
    {
        duration = 17.0f;
        powerupMultiplier *= 2;
        musicManager.powerUpOn = true;
        while (musicManager.powerUpOn)
        {
            if (duration <= 0)
            {
                if (PlayerPrefs.GetInt("Difficulty", 0) == 2)
                {
                    musicManager.playHardMusic();
                }
                else
                {
                    musicManager.playMainMusic();
                }
                LasersObject.SetActive(false);
                musicManager.powerUpOn = false;
            }
            Countdown();
            yield return new WaitForSeconds(1);
        }
        powerupMultiplier = 1;
        yield return null;
    }

    private void Countdown()
    {
        duration -= 1;
    }

    private void CountdownVisibility()
    {
        i -= 1;
    }

    private void ReloadCountdown()
    {
        reloadDuration -= 0.03f;
    }

    private IEnumerator ChangeLightColor()
    {
        previousIndex = -1;

        if (PlayerPrefs.GetInt("Difficulty") == 2)
        {
            directionalLight.intensity = 1.0f;
        }

        while (musicManager.powerUpOn)
        {
            index = Random.Range(0, colors.Length - 2);
            while (previousIndex == index)
            {
                index = Random.Range(0, colors.Length - 2);
            }
            previousIndex = index;
            directionalLight.color = colors[index];
            yield return new WaitForSeconds(0.75f);
        }

        directionalLight.color = colors[6];
        if (PlayerPrefs.GetInt("Difficulty") == 2)
        {
            directionalLight.intensity = 3.5f;
        }
        yield return null;
    }
}
