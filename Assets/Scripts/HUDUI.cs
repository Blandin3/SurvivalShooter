using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public Slider healthSlider;
    public PlayerHealth playerHealth;

    void OnEnable()
    {
        if (panel != null) panel.SetActive(false);

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("HUDUI.OnEnable(): GameManager.Instance is null, HUD will not update.", this);
            return;
        }

        GameManager.Instance.OnScoreChanged += UpdateScore;
        GameManager.Instance.OnTimeChanged += UpdateTime;
        GameManager.Instance.OnGameStarted += ShowPanel;
        GameManager.Instance.OnGameEnded += HidePanel;

        // Don't rely purely on catching the OnGameStarted event - if GameManager already
        // reached Playing before this object finished enabling (subscription-timing race),
        // sync to the current state directly instead of missing the moment forever.
        if (GameManager.Instance.State == GameState.Playing)
        {
            ShowPanel();
            UpdateScore(GameManager.Instance.Score);
            UpdateTime(GameManager.Instance.TimeRemaining);
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnTimeChanged -= UpdateTime;
            GameManager.Instance.OnGameStarted -= ShowPanel;
            GameManager.Instance.OnGameEnded -= HidePanel;
        }
    }

    void Update()
    {
        if (playerHealth != null && healthSlider != null)
        {
            healthSlider.value = playerHealth.currentHealth / playerHealth.maxHealth;
        }
    }

    void ShowPanel()
    {
        if (panel != null) panel.SetActive(true);
    }

    void HidePanel()
    {
        if (panel != null) panel.SetActive(false);
    }

    void UpdateScore(int score)
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    void UpdateTime(float time)
    {
        if (timeText != null) timeText.text = "Time: " + Mathf.CeilToInt(time);
    }
}
