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

    [Header("Music")]
    public AudioClip menuMusic;
    [Range(0f, 1f)] public float menuMusicVolume = 0.6f;

    bool level2Selected;
    AudioSource audioSource;

    void Start()
    {
        UpdateLevelToggleLabel();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.volume = menuMusicVolume;
            audioSource.spatialBlend = 0f; // background music should play at full volume regardless of listener distance
            audioSource.Play();
        }
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

    public void OnQuitPressed()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
