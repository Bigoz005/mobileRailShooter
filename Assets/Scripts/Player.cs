using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int score;
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

    const string SCORETEXT = "Score: ";

    void Start()
    {
        health = 3;
        score = 0;
        isDeath = false;
        textMesh = ScoreText.GetComponent<TextMeshProUGUI>();
        textMesh.SetText(SCORETEXT + score);
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

    public void AddHealth()
    {
        if (health == 3 || health >= 3)
        {
            AddScore(1000);
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
                HealthTexture1.SetActive(false);
                HealthTexture2.SetActive(false);
                HealthTexture3.SetActive(false);
                break;
            case 1:
                HealthTexture1.SetActive(true);
                HealthTexture2.SetActive(false);
                HealthTexture3.SetActive(false);
                break;
            case 2:
                HealthTexture1.SetActive(true);
                HealthTexture2.SetActive(true);
                HealthTexture3.SetActive(false);
                break;
            case 3:
                HealthTexture1.SetActive(true);
                HealthTexture2.SetActive(true);
                HealthTexture3.SetActive(true);
                break;
        }
    }

}
