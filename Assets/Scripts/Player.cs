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

    private int score;
    private int highScore;
    private int health;
    private bool isDeath;

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
    private TextMeshProUGUI textMesh2;

    const string SCORETEXT = "Score: ";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameplayCanvas.activeInHierarchy)
            {
                Time.timeScale = 0;
                gameplayCanvas.SetActive(false);
                pauseCanvas.SetActive(true);
            }
        }
    }

    void Start()
    {
        health = 3;
        score = 0;
        isDeath = false;
        textMesh = ScoreText.GetComponent<TextMeshProUGUI>();
        textMesh.SetText(SCORETEXT + score);
        highScore = PlayerPrefs.GetInt("HighScore");
    }

    public void AddScore(int scoreToAdd)
    {
        this.score += scoreToAdd;
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
        if (health == 0)
        {
            isDeath = true;
        }
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
                isDeath = true;
                if (score > highScore)
                {
                    PlayerPrefs.SetInt("HighScore", score);
                    PlayerPrefs.Save();
                }
                textMesh2 = ScoreText2.GetComponent<TextMeshProUGUI>();
                Debug.Log(this.GetScore());
                textMesh2.SetText("Score: " + this.GetScore());
                gameOverCanvas.SetActive(true);
                gameplayCanvas.SetActive(false);
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
