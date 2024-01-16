using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject gameplayCanvas;
    [SerializeField]
    private GameObject pauseCanvas;
    [SerializeField]
    private GameObject gameOverCanvas;

    public int controlsScoreDividor = 5;
    private int points = 1000;
    private int score;
    private int highScore;
    private int health;

    [SerializeField]
    private GameObject HealthTexture1;
    [SerializeField]
    private GameObject HealthTexture2;
    [SerializeField]
    private GameObject HealthTexture3;
    [SerializeField]
    private GameObject ScoreText;
    private TextMeshProUGUI textMesh;

    [SerializeField]
    private GameObject ScoreText2;
    [SerializeField]
    private LaunchEnemies launchEnemies;
    private TextMeshProUGUI textMesh2;

    private GameObject musicManager;
    private GameObject soundPlayer;
    private GameObject enemyPlayer;

    const string SCORETEXT = "Score: ";

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
        health = 3;
        score = 0;
        textMesh = ScoreText.GetComponent<TextMeshProUGUI>();
        textMesh.SetText(SCORETEXT + score);
        highScore = PlayerPrefs.GetInt("HighScore");
    }

    public void AddScore(int scoreToAdd)
    {
        this.score += scoreToAdd;
        if (this.score < 0)
        {
            this.score = 0;
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

    public void CheckHealth()
    {
        switch (health)
        {
            case 0:
                if (score > highScore)
                {
                    PlayerPrefs.SetInt("HighScore", score);
                    PlayerPrefs.Save();
                }
                textMesh2 = ScoreText2.GetComponent<TextMeshProUGUI>();
                textMesh2.SetText("Score: " + this.GetScore());
                gameOverCanvas.SetActive(true);
                gameplayCanvas.SetActive(false);
                Time.timeScale = 0;
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
