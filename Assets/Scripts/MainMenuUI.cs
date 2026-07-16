using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject leaderboardPanel;
    public string gameSceneName = "Game";

    public void OnStartPressed()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnLeaderboardPressed()
    {
        if (leaderboardPanel != null) leaderboardPanel.SetActive(true);
    }

    public void OnCloseLeaderboardPressed()
    {
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
    }

    public void OnEasySelected()
    {
        DifficultySettings.Current = Difficulty.Easy;
    }

    public void OnNormalSelected()
    {
        DifficultySettings.Current = Difficulty.Normal;
    }

    public void OnHardSelected()
    {
        DifficultySettings.Current = Difficulty.Hard;
    }
}
