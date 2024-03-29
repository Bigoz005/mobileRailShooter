using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> scores;
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
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Leaderboards.Gnomes.GetEntries(Dan.Models.LeaderboardSearchQuery.ByTimePeriod(Dan.Enums.TimePeriodType.AllTime), ((msg) =>
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
                    if (msg == null || msg[0].Score != 0)
                    {
                        scores[0].text = "Fetching...";
                    }

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

                        if(i == j)
                        {
                            fetched = true;
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
        if (PlayerPrefs.GetString("Username").Equals("----") || PlayerPrefs.GetString("Username", "").Equals(""))
        {
            resetNicknameText();
        }
        else
        {
            textMesh.SetText("Highscore: " + PlayerPrefs.GetInt("Highscore", 0));
        }
    }

    public void resetNicknameText()
    {
        textMesh.SetText("Start game and enter username");
    }

    public IEnumerator WaitAndLoad()
    {
        while (!fetched)
        {
            yield return new WaitForSeconds(1f);
            UpdateTable();
            yield return null;
        }
        yield return null;
    }
}
