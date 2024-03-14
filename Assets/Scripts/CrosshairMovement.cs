using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CrosshairMovement : MonoBehaviour
{

    [SerializeField]
    private GameObject joystick;
    [SerializeField]
    private GameObject self;
    [SerializeField]
    private GameObject reloadCircle;
    [SerializeField]
    private GameObject shootButton;
    [SerializeField]
    private GameObject gameplayCanvas;
    [SerializeField]
    private GameObject gameOverCanvas;
    [SerializeField]
    private AudioClip shootClip;
    [SerializeField]
    private AudioClip bonusClip;
    [SerializeField]
    private AudioClip healthClip;
    [SerializeField]
    private AudioClip powerUpClip;
    [SerializeField]
    private AudioClip gnomeClip;
    [SerializeField]
    private AudioClip targetClip;
    [SerializeField]
    private GameObject LasersObject;
    [SerializeField]
    private Camera circleCamera;
    [SerializeField]
    private GameObject gun;
    [SerializeField]
    private Material[] materials;

    private Camera cam;
    private Vector3 previousPosition = new Vector3(0, 0, 0);
    private AudioSource audioSource;
    private AudioSource enemyAudioSource;
    private MusicManager musicManager;
    private string[] layerNames = { "RayCast", "Specials" };
    private float previousTime;
    private float actualTime;
    private ParticleSystem particle = null;
    private Color baseColor = new(1, 1, 1, 1);

    private float duration = 17.0f;
    private bool isSwapingActive = false;

    private int points;
    private bool touchControlEnabled;
    private int i = 1;
    private int previousIndex;
    private int index;
    private int powerupMultiplier = 1;
    private bool isReloding = false;
    private int controlsScoreDividor = 5;
    private readonly Color[] colors = { Color.blue, Color.green, Color.red, Color.yellow };


    private void Start()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "EasyScene":
                baseColor = new(0.85f, 0.85f, 0.85f, 1);
                break;
            case "MediumScene":
                baseColor = new(0.65f, 0.65f, 0.65f, 1);
                break;
            case "HardScene":
                baseColor = new(0.36f, 0.36f, 0.36f, 1);
                break;
        }

        cam = Camera.main;
        audioSource = GameObject.FindGameObjectWithTag("SoundPlayer").GetComponent<AudioSource>();
        enemyAudioSource = GameObject.FindGameObjectWithTag("EnemyPlayer").GetComponent<AudioSource>();
        musicManager = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicManager>();
        points = cam.GetComponent<Player>().GetPoints();
        points = points * (PlayerPrefs.GetInt("Difficulty", 0) + 1);
        previousTime = Time.deltaTime;
        actualTime = previousTime;

        if (PlayerPrefs.GetInt("Controls") == 1)
        {
            touchControlEnabled = true;
            joystick.transform.parent.parent.gameObject.SetActive(false);
            controlsScoreDividor = 5;
            shootButton.SetActive(false);
            self.GetComponent<Image>().enabled = false;
        }
        else
        {
            touchControlEnabled = false;
            controlsScoreDividor = 1;
            joystick.transform.parent.gameObject.SetActive(true);
            shootButton.SetActive(true);
            self.GetComponent<Image>().enabled = true;
        }
    }

    void Update()
    {
        actualTime += Time.deltaTime;
        if (!touchControlEnabled)
        {
            if (previousPosition.x != joystick.transform.localPosition.x || previousPosition.y != joystick.transform.localPosition.y)
            {
                previousPosition = joystick.transform.localPosition;
                self.transform.localPosition = new Vector3(previousPosition.x * 7.3f, previousPosition.y * 4.5f, previousPosition.z);
                reloadCircle.transform.localPosition = self.transform.localPosition;
            }
        }
        else
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                checkShoot();
            }
        }
    }
    public void checkShoot()
    {
        Ray ray = new();

        if (touchControlEnabled)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ray = cam.ScreenPointToRay(Input.GetTouch(0).position);

                if (!isReloding)
                {
                    Vector2 anchoredPosition;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(reloadCircle.transform.parent.transform.parent.GetComponent<RectTransform>(), Input.GetTouch(0).position, circleCamera, out anchoredPosition);
                    reloadCircle.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
                }
            }
        }
        else
        {
            Vector3 crosshairPosition = self.transform.localPosition;
            crosshairPosition.z = 100000000000f;
            crosshairPosition = Camera.main.ScreenToWorldPoint(crosshairPosition);

            gun.transform.LookAt(self.transform);
            ray = new Ray(gun.transform.position, gun.transform.forward * 10000000);
        }

        checkShoot(ray);
    }

    private void checkShoot(Ray ray)
    {

        RaycastHit hit;


        if (!musicManager.powerUpOn)
        {
            points = cam.GetComponent<Player>().GetPoints();
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
                            tempPoints = powerupMultiplier * (points / 10 / controlsScoreDividor);
                        }
                        else
                        {
                            tempPoints = points / 10 / controlsScoreDividor;
                        }

                        cam.gameObject.GetComponent<Player>().AddScore(tempPoints);

                        if (hit.collider.name.Contains("Hard"))
                        {
                            hit.collider.gameObject.GetComponent<EnemyHard>().StopAllGnomeCoroutines();
                            hit.collider.gameObject.GetComponent<EnemyHard>().enabled = false;
                            hit.collider.gameObject.transform.GetChild(3).gameObject.SetActive(true);
                            hit.collider.gameObject.transform.GetChild(4).GetComponent<TextMeshPro>().SetText("+ " + tempPoints);
                            hit.collider.gameObject.transform.GetChild(4).gameObject.SetActive(true);
                            StartCoroutine(PointsTextVisibility(hit.collider.gameObject.transform.GetChild(4).gameObject));
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
                            StartCoroutine(PointsTextVisibility(hit.collider.gameObject.transform.GetChild(7).gameObject));
                            hit.collider.gameObject.tag = "Untagged";
                        }
                        enemyAudioSource.Play();

                        break;
                    case "ScorePowerUp":
                        particle = hit.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                        hit.collider.gameObject.transform.GetChild(1).GetComponent<TextMeshPro>().SetText("+ " + points / controlsScoreDividor);
                        hit.collider.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                        StartCoroutine(PointsTextVisibility(hit.collider.gameObject.transform.GetChild(1).gameObject));
                        hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                        if (particle.isPlaying)
                        {
                            particle.Stop();
                        }
                        particle.Play();

                        audioSource.clip = bonusClip;
                        cam.gameObject.GetComponent<Player>().AddScore(points / controlsScoreDividor);
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
                        bool i = cam.gameObject.GetComponent<Player>().AddHealth(points / 2 / controlsScoreDividor, hit.collider.gameObject.transform.GetChild(1).gameObject);
                        if (i)
                        {
                            hit.collider.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                            StartCoroutine(PointsTextVisibility(hit.collider.gameObject.transform.GetChild(1).gameObject));
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
                        musicManager.powerUpOn = true;
                        PowerUpDuration();
                        StartCoroutine(ChangeMaterialsColor());
                        break;
                    case "Target":

                        tempPoints = powerupMultiplier * points / 10 / controlsScoreDividor / 2;

                        audioSource.clip = targetClip;
                        cam.gameObject.GetComponent<Player>().AddScore(tempPoints);
                        hit.collider.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().SetText("+ " + tempPoints);
                        hit.collider.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        StartCoroutine(PointsTextVisibility(hit.collider.gameObject.transform.GetChild(0).gameObject));
                        break;
                }
            }
            self.GetComponent<Image>().enabled = false;
            Reload();
            if (!(enemyAudioSource.clip == gnomeClip && enemyAudioSource.isPlaying))
            {
                audioSource.Play();
            }
        }
    }

    private IEnumerator PointsTextVisibility(GameObject gameObject)
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
    private async void Reload()
    {
        try
        {
            Image image = reloadCircle.GetComponent<Image>();
            image.enabled = true;
            isReloding = true;
            if (!touchControlEnabled)
            {
                self.GetComponent<Image>().enabled = false;
            }

            while (image.fillAmount > 0)
            {
                while (Time.timeScale == 0)
                {
                    await Task.Delay(200);
                }

                ReloadCountdown(image);
                if (SceneManager.GetActiveScene().name.Equals("HardScene"))
                {
                    await Task.Delay(20);
                }
                else
                {
                    await Task.Delay(30);
                }
            }
            if (image.fillAmount <= 0.1)
            {
                image.fillAmount = 1;
                isReloding = false;
            }
            if (PlayerPrefs.GetInt("Controls") != 1)
            {
                self.GetComponent<Image>().enabled = true;
            }
            image.enabled = false;
            if (!touchControlEnabled)
            {
                self.GetComponent<Image>().enabled = true;
            }
            await Task.Yield();
        }
        catch (MissingReferenceException)
        {

            await Task.Yield();
        }
    }

    private async void PowerUpDuration()
    {
        try
        {
            duration = 17.0f;
            powerupMultiplier *= 2;
            while (musicManager.powerUpOn)
            {
                while (Time.timeScale == 0)
                {
                    await Task.Delay(200);
                }

                if (duration <= 0)
                {
                    if (PlayerPrefs.GetInt("Difficulty", 0) == 2)
                    {
                        musicManager.playHardMusic();
                    }
                    else
                    if (PlayerPrefs.GetInt("Difficulty", 0) == 1)
                    {
                        musicManager.playMediumMusic();
                    }
                    else
                    {
                        musicManager.playMainMusic();
                    }
                    LasersObject.SetActive(false);
                    musicManager.powerUpOn = false;
                }
                Countdown();
                await Task.Delay(1000);
            }
            powerupMultiplier = 1;
            await Task.Yield();
        }
        catch (MissingReferenceException)
        {
            /*Debug.Log("Power Up Duration: " + e);*/
            await Task.Yield();
        }
    }

    private void Countdown()
    {
        duration -= 1;
    }

    private void CountdownVisibility()
    {
        i -= 1;
    }

    private void ReloadCountdown(Image image)
    {
        image.fillAmount -= 0.1f;
    }

    /*private async void ChangeLightColor()
    {
        try
        {
            previousIndex = -1;

            if (PlayerPrefs.GetInt("Difficulty") == 2)
            {
                directionalLight.intensity = 1.0f;
            }

            while (musicManager.powerUpOn)
            {
                while (Time.timeScale == 0)
                {
                    await Task.Delay(200);
                }

                index = Random.Range(0, colors.Length - 2);
                while (previousIndex == index)
                {
                    index = Random.Range(0, colors.Length - 2);
                }
                previousIndex = index;
                directionalLight.color = colors[index];
                await Task.Delay(750);
            }

            directionalLight.color = colors[6];
            if (PlayerPrefs.GetInt("Difficulty") == 2)
            {
                directionalLight.intensity = 3.5f;
            }
            await Task.Yield();

        }
        catch (MissingReferenceException e)
        {
            Debug.Log("Change Light Color: " + e);
            await Task.Yield();
        }
    }*/

    public IEnumerator ChangeMaterialsColor()
    {
        if (!isSwapingActive)
        {
            isSwapingActive = true;
            while (musicManager.powerUpOn)
            {
                while (Time.timeScale == 0)
                {
                    yield return new WaitForSeconds(0.3f);
                }

                index = Random.Range(0, colors.Length - 1);
                while (previousIndex == index)
                {
                    index = Random.Range(0, colors.Length - 1);
                }
                previousIndex = index;

                foreach (Material mat in materials)
                {
                    mat.color = colors[index];
                }

                yield return new WaitForSeconds(0.75f);
            }

            foreach (Material mat in materials)
            {
                mat.color = baseColor;
            }

            isSwapingActive = false;
        }
        yield return null;
    }

    public void turnOffPowerUp()
    {
        musicManager.powerUpOn = false;
        if (SceneManager.GetActiveScene().name.Equals("HardScene"))
        {
            musicManager.playHardMusic();
        }
        else
        if (SceneManager.GetActiveScene().name.Equals("MediumScene"))
        {
            musicManager.playMediumMusic();
        }
        else
        {
            musicManager.playMainMusic();
        }
        LasersObject.SetActive(false);
    }
}
