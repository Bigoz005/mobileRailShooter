using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject optionCanvas;
    [SerializeField] private GameObject usernameCanvas;
    [SerializeField] private GameObject highScoreText;
    [SerializeField] private GameObject playerNicknameText;
    [SerializeField] private GameObject gnome1;
    [SerializeField] private GameObject gnome2;
    [SerializeField] private GameObject internetConnection;

    private TextMeshProUGUI textMesh;
    private const string SCORETEXT = "Highscore: ";

    private AsyncOperation _asyncOperation;

    public void Awake()
    {
        RefreshHighscore();
    }

    public void RefreshHighscore()
    {
        textMesh = highScoreText.GetComponent<TextMeshProUGUI>();
        textMesh.SetText(SCORETEXT + PlayerPrefs.GetInt("HighScore", 0) + " (Global Rank: " + PlayerPrefs.GetInt("Rank", 0) + ")");
    }

    public void SetNickname()
    {
        string username = playerNicknameText.GetComponent<TextMeshProUGUI>().text;

        if (username.Length != 1 && !username.Equals(""))
        {
            PlayerPrefs.SetString("Username", username);
            StartGame();
        }
    }

    public void CheckIfReadyToPlay()
    {
        string user = PlayerPrefs.GetString("Username");
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            internetConnection.SetActive(true);
        }
        else
        {
            if (user.Equals("----") || user.Equals("") || user == null)
            {
                usernameCanvas.SetActive(true);
                mainMenuCanvas.SetActive(false);
            }
            else
            {
                StartGame();
            }
        }

    }

    public void ResetNickname()
    {
        PlayerPrefs.SetString("Username", "----");
        GetComponent<Options>().username.text = "Username: " + PlayerPrefs.GetString("Username", "----");
    }

    public void ShowOptions()
    {
        mainMenuCanvas.SetActive(false);
        optionCanvas.SetActive(true);
        if (gnome1 != null && gnome2 != null)
        {
            gnome1.SetActive(false);
            gnome2.SetActive(false);
        }
    }

    public void ShowMenuFromUsername()
    {
        internetConnection.SetActive(false);
        usernameCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);

    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        int difficulty = PlayerPrefs.GetInt("Difficulty");
        string sceneName = "";
        switch (difficulty)
        {
            case 0:
                sceneName = "EasyScene";
                break;
            case 1:
                sceneName = "MediumScene";
                break;
            case 2:
                GameObject musicManager = GameObject.FindGameObjectWithTag("MusicManager");
                musicManager.GetComponent<AudioSource>().clip = musicManager.GetComponent<MusicManager>().HardMusic;
                musicManager.GetComponent<AudioSource>().Play();
                sceneName = "HardScene";
                break;
        }
        LoadSceneAsyncProcess(sceneName);
    }

    private async void LoadSceneAsyncProcess(string sceneName)
    {
        this._asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        mainMenuCanvas.transform.GetChild(1).GetComponent<Button>().interactable = false;
        mainMenuCanvas.transform.GetChild(2).GetComponent<Button>().interactable = false;
        mainMenuCanvas.transform.GetChild(3).GetComponent<Button>().interactable = false;

        this._asyncOperation.allowSceneActivation = false;

        while (!this._asyncOperation.isDone)
        {
            if (this._asyncOperation.progress >= 0.89)
            {
                mainMenuCanvas.transform.GetChild(1).GetComponent<Button>().interactable = true;
                mainMenuCanvas.transform.GetChild(2).GetComponent<Button>().interactable = true;
                mainMenuCanvas.transform.GetChild(3).GetComponent<Button>().interactable = true;
                this._asyncOperation.allowSceneActivation = true;
            }
            await System.Threading.Tasks.Task.Yield();
        }

        await System.Threading.Tasks.Task.Yield();
    }
}
