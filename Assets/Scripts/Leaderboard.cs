using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> scores;
    private string publicLeaderboardKey = "adc4cd6ac33116a538d58e21c4db09a652d82bc8884da92c97f91b82bb1bac37";
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
                typeText.text = "New Users (Week)";
                break;
            case 2:
                filter = Dan.Enums.TimePeriodType.Today;
                typeText.text = "New Users (Today)";
                break;
        }

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, Dan.Models.LeaderboardSearchQuery.ByTimePeriod(filter),
            ((msg) =>
            {
                foreach (TextMeshProUGUI score in scores)
                {
                    score.text = "";
                }

                int j = 10;

                if (msg.Length < j)
                {
                    j = msg.Length + 1;
                }
                try
                {
                    for (int i = 1; i <= j; ++i)
                    {
                        if (msg[i - 1].Username.Length > 12)
                        {
                            scores[i - 1].text = (i) + ". " + (msg[i - 1].Username.Substring(0, 12) + ": " + msg[i - 1].Score.ToString());
                        }
                        else
                        {
                            scores[i - 1].text = (i) + ". " + (msg[i - 1].Username + ": " + msg[i - 1].Score.ToString());
                        }
                    }
                }
                catch (System.IndexOutOfRangeException)
                {
                    scores[0].text = "Failed to fetch data";
                }
            }),
            ((error) =>
            {
                scores[0].text = "Failed to fetch data";
            }));

            LeaderboardCreator.GetPersonalEntry(publicLeaderboardKey, (msg) =>
            {
                int highscore = PlayerPrefs.GetInt("HighScore", 0);
                if (msg.Score < highscore || (!msg.Username.Equals(PlayerPrefs.GetString("Username")) && !PlayerPrefs.GetString("Username").Equals("----")))
                {
                    SetLeaderboardEntry(highscore);
                    fetched = false;
                }
                else
                {
                    fetched = true;
                    textMesh.SetText("Highscore: " + msg.Score + " (Global Rank: " + msg.Rank + ")");
                }
            },
            (error) =>
            {
                textMesh.SetText("Highscore: " + PlayerPrefs.GetInt("HighScore", 0) + " (Global Rank: " + PlayerPrefs.GetInt("Rank", 0) + ")\nFailed to fetch data");
            });
        }
        else
        {
            scores[0].text = "Failed to fetch data";
            textMesh.SetText("Highscore: " + PlayerPrefs.GetInt("HighScore", 0) + " (Global Rank: " + PlayerPrefs.GetInt("Rank", 0) + ")\nFailed to fetch data");
            fetched = true;
        }
    }

    public void SetLeaderboardEntry(int score)
    {
        LeaderboardCreator.DeleteEntry(publicLeaderboardKey);

        string tempUsername = PlayerPrefs.GetString("Username");
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, tempUsername, score, ((msg) =>
        {
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
            yield return null;
        }
        yield return null;
    }
}
