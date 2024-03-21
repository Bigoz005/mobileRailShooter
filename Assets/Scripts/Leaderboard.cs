using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> scores;
    private int type = 0;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI textMesh;
    private bool fetched = false;

    private void Awake()
    {
        fetched = false;
        StartCoroutine(WaitAndLoad());
    }

    public void UpdateTable()
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
            Leaderboards.Gnomes.GetEntries(Dan.Models.LeaderboardSearchQuery.ByTimePeriod(filter), ((msg) =>
            {
                foreach (TextMeshProUGUI score in scores)
                {
                    score.text = "";
                }

                int j = 10;

                if (msg.Length < j)
                {
                    j = msg.Length;
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
        }
    }

    public void GetLeaderboard()
    {
        int highscore = PlayerPrefs.GetInt("HighScore", 0);

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (PlayerPrefs.GetString("Username").Equals("----") || PlayerPrefs.GetString("Username").Equals(""))
            {
                Leaderboards.Gnomes.DeleteEntry();
                Leaderboards.Gnomes.ResetPlayer();
                textMesh.SetText("Start game and enter username");
            }
            else
            {
                Leaderboards.Gnomes.GetPersonalEntry((msg) =>
                {
                    if (msg.Score >= highscore)
                    {
                        textMesh.SetText("Highscore: " + msg.Score + " (Global Rank: " + msg.Rank + ")");
                        PlayerPrefs.SetInt("Highscore", msg.Score);
                    }
                    else
                    {
                        Leaderboards.Gnomes.UploadNewEntry(PlayerPrefs.GetString("Username"), highscore, (msg2) =>
                        {
                            Leaderboards.Gnomes.GetPersonalEntry((msg3) =>
                            {
                                textMesh.SetText("Highscore: " + msg3.Score + " (Global Rank: " + msg3.Rank + ")");
                            });
                        }, (err) =>
                        {
                            Leaderboards.Gnomes.DeleteEntry();
                            Leaderboards.Gnomes.ResetPlayer();
                            Leaderboards.Gnomes.UploadNewEntry(PlayerPrefs.GetString("Username"), highscore, (msg4) =>
                            {
                                Leaderboards.Gnomes.GetPersonalEntry((msg5) =>
                                {
                                    textMesh.SetText("Highscore: " + msg5.Score + " (Global Rank: " + msg5.Rank + ")");
                                });
                            });
                        });
                    }
                }, (error) =>
               {
                   textMesh.SetText("Highscore: " + PlayerPrefs.GetInt("HighScore", 0) + "\nFailed to fetch data");
               });
            }
            fetched = true;
        }
        else
        {
            scores[0].text = "Failed to fetch data";
            textMesh.SetText("Highscore: " + PlayerPrefs.GetInt("HighScore", 0) + "\nFailed to fetch data");
        }

        UpdateTable();
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
