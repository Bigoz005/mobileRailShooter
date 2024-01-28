using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> scores;
    private string publicLeaderboardKey = "40d376740a12a143deb03173d5b29b01af5a340378b7979348016cc644ad0577";
    private int type = 0;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI textMesh;
    private bool fetched = false;

    private void Awake()
    {
        fetched = false;
        StartCoroutine(WaitAndLoad());
    }

    public void GetLeaderboard()
    {
        Dan.Enums.TimePeriodType filter = Dan.Enums.TimePeriodType.AllTime;
        switch (type)
        {
            case 0:
                filter = Dan.Enums.TimePeriodType.AllTime;
                typeText.text = "All Time";
                break;
            case 1:
                filter = Dan.Enums.TimePeriodType.ThisWeek;
                typeText.text = "This Week";
                break;
            case 2:
                filter = Dan.Enums.TimePeriodType.Today;
                typeText.text = "Today";
                break;
        }

        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, Dan.Models.LeaderboardSearchQuery.ByTimePeriod(filter),
        ((msg) =>
        {
            int j = 9;
            if (msg.Length < j)
                j = msg.Length;
            
            for (int i = 0; i < j; ++i)
            {

                if (msg[i].Username.Length > 12)
                {
                    scores[i].text = (i + 1) + ". " + (msg[i].Username.Substring(0, 12) + ": " + msg[i].Score.ToString());
                }
                else
                {
                    scores[i].text = (i + 1) + ". " + (msg[i].Username + ": " + msg[i].Score.ToString());
                }
            }
        }),
        ((error) =>
        {
            scores[0].text = "Failed to fetch data";
        }));

        LeaderboardCreator.GetPersonalEntry(publicLeaderboardKey, (msg) =>
        {
            fetched = true;
            PlayerPrefs.SetInt("HighScore", msg.Score);
            PlayerPrefs.SetInt("Rank", msg.Rank);
            textMesh.SetText("Highscore: " + msg.Score + " (Global Rank: " + msg.Rank + ")");
        },
        (error) =>
        {
            textMesh.SetText("Highscore: " + PlayerPrefs.GetInt("HighScore", 0) + " (Global Rank: " + PlayerPrefs.GetInt("Rank", 0) + ")\nLast knew data-failed to fetch");
        });
    }

    public void SetLeaderboardEntry(int score)
    {
        string tempUsername = PlayerPrefs.GetString("Username", "----");

        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, tempUsername, score, ((msg) =>
        {
            GetLeaderboard();
        }));
    }

    public void IncreaseType()
    {
        type++;
        if (type > 2)
        {
            type = 0;
        }
        GetLeaderboard();
    }

    public void DecreaseType()
    {
        type--;
        if (type < 0)
        {
            type = 2;
        }
        GetLeaderboard();
    }

    public IEnumerator WaitAndLoad()
    {
        while (!fetched)
        {
            yield return new WaitForSeconds(1f);
            GetLeaderboard();
        }

        yield return null;
    }
}
