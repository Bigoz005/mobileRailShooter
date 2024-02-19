using TMPro;
using UnityEngine;
using Dan.Main;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    private int points = 1000;
    private int score;
    private int highScore;
    private int health;
    private string publicLeaderboardKey = "adc4cd6ac33116a538d58e21c4db09a652d82bc8884da92c97f91b82bb1bac37";

    [SerializeField] private GameObject HealthTexture1;
    [SerializeField] private GameObject HealthTexture2;
    [SerializeField] private GameObject HealthTexture3;
    [SerializeField] private GameObject ScoreText;
    [SerializeField] private GameObject ScoreText2;
    [SerializeField] private LaunchEnemies launchEnemies;
    [SerializeField] private GameObject rewardedButton;

    private TextMeshProUGUI textMesh;
    private TextMeshProUGUI textMesh2;

    private GameObject musicManager;
    private GameObject soundPlayer;
    private GameObject enemyPlayer;

    public InterstitialAd interstitialAd;
    public RewardedAd rewardedAd;
    private const string SCORETEXT = "Score: ";
    public bool isRewarded;

    float tick = 0f;
    private Color grayColor = new(0.5f, 0.5f, 0.5f);
    private Color fullColor = new(1f, 1f, 1f);
    private Color blinkingColor = new(0.5f, 1f, 1f);
    private Color blinkSecondColor = new(0.1f, 1f, 0.1f);

    public int GetPoints()
    {
        return points;
    }

    public void SetPoints(float pointsToAssign)
    {
        points = (int)pointsToAssign;
    }

    public int GetHealth()
    {
        return health;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenMenu();
        }

        if (!isRewarded)
        {
            tick += Time.unscaledDeltaTime * 2;
            rewardedButton.GetComponent<Image>().color = Color.Lerp(blinkSecondColor, blinkingColor, Mathf.PingPong(tick, 1));
        }
    }

    public void OpenMenu()
    {
        musicManager = GameObject.FindGameObjectWithTag("MusicManager");
        soundPlayer = GameObject.FindGameObjectWithTag("SoundPlayer");
        enemyPlayer = GameObject.FindGameObjectWithTag("EnemyPlayer");
        if (gameplayCanvas.activeInHierarchy)
        {
            Time.timeScale = 0;
            Camera.main.gameObject.GetComponent<SystemPreferences>().IsPaused = true;

            musicManager.GetComponent<AudioSource>().Pause();
            soundPlayer.GetComponent<AudioSource>().Pause();
            enemyPlayer.GetComponent<AudioSource>().Pause();

            gameplayCanvas.SetActive(false);
            pauseCanvas.SetActive(true);
        }
    }

    void Start()
    {
        GameObject adsManager = GameObject.FindGameObjectWithTag("AdsManager");
        health = 3;
        score = 0;
        textMesh = ScoreText.GetComponent<TextMeshProUGUI>();
        textMesh.SetText(SCORETEXT + score);
        highScore = PlayerPrefs.GetInt("HighScore");
        interstitialAd = adsManager.GetComponent<InterstitialAd>();
        rewardedAd = adsManager.GetComponent<RewardedAd>();
        isRewarded = false;
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        if (score < 0)
        {
            score = 0;
        }
        textMesh.SetText(SCORETEXT + score);
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
        textMesh.SetText(SCORETEXT + score);
    }

    public void GetHit()
    {
        health -= 1;
        CheckHealth();
    }

    public bool AddHealth(int scoreToAdd, GameObject gameobject)
    {
        bool i = false;
        if (health >= 3)
        {
            AddScore(scoreToAdd);
            gameobject.GetComponent<TextMeshPro>().SetText("+ " + scoreToAdd);
            i = true;
        }
        else
        {
            health += 1;
        }
        CheckHealth();
        return i;
    }

    public void AddHealth(int scoreToAdd)
    {
        if (health >= 3)
        {
            AddScore(scoreToAdd);
        }
        else
        {
            health += 1;
        }
        CheckHealth();
    }

    public void ShowRewardAd()
    {
        rewardedAd.ShowAd();
    }

    public void ShowInterAd()
    {
        interstitialAd.ShowAd();
    }

    public void ResumeGame()
    {
        try
        {
            health = 1;
            isRewarded = true;
            gameOverCanvas.SetActive(false);
            gameplayCanvas.SetActive(true);
            rewardedButton.GetComponent<Button>().interactable = false;
            rewardedButton.GetComponent<Image>().color = grayColor;
            Time.timeScale = 1;
        }
        catch (MissingReferenceException e)
        {
            Debug.Log(e.StackTrace);
        }

    }

    public void CheckHealth()
    {
        switch (health)
        {
            case 0:
                if (!isRewarded)
                {
                    rewardedAd.player = this;
                    rewardedAd._showAdButton = rewardedButton;
                    rewardedButton.GetComponent<Image>().color = fullColor;
                    rewardedButton.GetComponent<Button>().interactable = true;
                }
                else
                {
                    ShowInterAd();
                }

                if (score > highScore)
                {
                    PlayerPrefs.SetInt("HighScore", score);
                    PlayerPrefs.Save();
                }
                textMesh2 = ScoreText2.GetComponent<TextMeshProUGUI>();
                textMesh2.SetText("Score: " + this.GetScore());

                if (interstitialAd.wasShowed)
                {
                    interstitialAd.wasPlayedOnGameOver = true;
                    interstitialAd.wasShowed = false;
                }
                gameOverCanvas.SetActive(true);
                gameplayCanvas.SetActive(false);
                Time.timeScale = 0;

                LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, PlayerPrefs.GetString("Username"), GetScore(), ((msg) =>
                {
                }));

                break;
            case 1:
                HealthTexture2.SetActive(false);
                HealthTexture3.SetActive(false);
                break;
            case 2:
                HealthTexture2.SetActive(true);
                HealthTexture3.SetActive(false);
                break;
            case 3:
                HealthTexture2.SetActive(true);
                HealthTexture3.SetActive(true);
                break;
        }
    }
}
