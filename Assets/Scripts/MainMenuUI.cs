using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject leaderboardPanel;
    public string level1SceneName = "Level_01";
    public string level2SceneName = "Level_02";

    [Tooltip("The label on the Difficulty/Level toggle button - shows which level is currently selected.")]
    public TMP_Text levelToggleLabel;

    bool level2Selected;

    void Start()
    {
        UpdateLevelToggleLabel();
    }

    public void OnLevelTogglePressed()
    {
        level2Selected = !level2Selected;
        UpdateLevelToggleLabel();
    }

    void UpdateLevelToggleLabel()
    {
        if (levelToggleLabel != null)
        {
            levelToggleLabel.text = level2Selected ? "Level 2" : "Level 1";
        }
    }

    public void OnStartPressed()
    {
        SceneManager.LoadScene(level2Selected ? level2SceneName : level1SceneName);
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
