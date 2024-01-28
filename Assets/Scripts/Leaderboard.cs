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

    private void Awake()
    {
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
            for (int i = 0; i < 9; ++i)
            {
                scores[i].text = (i+1) + ". " + (msg[i].Username + ": " + msg[i].Score.ToString());
            }
        }),
        ((error) =>
        {
            scores[0].text = "Failed to fetch data";
        }));

        LeaderboardCreator.GetPersonalEntry(publicLeaderboardKey, (msg) =>
        {
            PlayerPrefs.SetInt("HighScore", msg.Score);
            PlayerPrefs.SetInt("Rank", msg.Rank);
        });
    }

    public void SetLeaderboardEntry(int score)
    {
        string tempUsername = PlayerPrefs.GetString("Username", "----");

        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, tempUsername, score, ((msg) =>
        {
            GetLeaderboard();
        }));

        //TODO: Jesli taki nick juz istnieje to zastap wynik - mozliwe ze trzeba usunac wpis i dodac nowy - narazie pozwolono na wiele wpisow

        /*LeaderboardCreator.UpdateEntryUsername(publicLeaderboardKey, tempUsername, ((msg) =>
        {
            GetLeaderboard();
        }));*/
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
        while(scores[0].text.Length == 0) { 
            yield return new WaitForSeconds(1f);
            GetLeaderboard();
            textMesh.SetText("Highscore: " + PlayerPrefs.GetInt("HighScore", 0) + " (Global Rank: " + PlayerPrefs.GetInt("Rank", 0) + ")");
        }
        yield return null;
    }
}
