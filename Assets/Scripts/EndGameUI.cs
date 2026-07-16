using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class EndGameUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text finalScoreText;
    public TMP_Text enemiesDefeatedText;
    public TMP_Text timeSurvivedText;
    public string mainMenuSceneName = "MainMenu";

    void OnEnable()
    {
        if (panel != null) panel.SetActive(false);

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("EndGameUI.OnEnable(): GameManager.Instance is null, End Game panel will not show.", this);
            return;
        }

        GameManager.Instance.OnGameEnded += ShowSummary;
        GameManager.Instance.OnGameStarted += HidePanel;

        // Don't rely purely on catching the OnGameEnded event - if the game already ended
        // before this object finished enabling (subscription-timing race), sync to the
        // current state directly instead of missing the moment forever. GameManager persists
        // across scene reloads (DontDestroyOnLoad), so on a fresh Restart its State is still
        // GameOver for a moment until GameSceneStarter's Start() flips it back to Playing -
        // HidePanel() being wired to OnGameStarted is what clears this again a moment later.
        if (GameManager.Instance.State == GameState.GameOver)
        {
            ShowSummary();
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameEnded -= ShowSummary;
            GameManager.Instance.OnGameStarted -= HidePanel;
        }
    }

    void HidePanel()
    {
        if (panel != null) panel.SetActive(false);
    }

    void ShowSummary()
    {
        Debug.Log("EndGameUI.ShowSummary(): showing End Game panel (panel field is " + (panel != null ? panel.name : "NULL") + ").", this);

        if (panel != null) panel.SetActive(true);

        var gm = GameManager.Instance;
        if (gm == null) return;

        if (finalScoreText != null) finalScoreText.text = "Final Score: " + gm.Score;
        if (enemiesDefeatedText != null) enemiesDefeatedText.text = "Enemies Defeated: " + gm.EnemiesDefeated;
        if (timeSurvivedText != null) timeSurvivedText.text = "Time Survived: " + Mathf.CeilToInt(gm.matchDuration - gm.TimeRemaining) + "s";
    }

    public void OnRestartPressed()
    {
        StartCoroutine(LoadSceneAfterARSessionShutdown(SceneManager.GetActiveScene().name));
    }

    public void OnMainMenuPressed()
    {
        StartCoroutine(LoadSceneAfterARSessionShutdown(mainMenuSceneName));
    }

    // The Editor's XR Simulation camera provider doesn't always cleanly shut down when its
    // AR Session GameObject is destroyed in the same instant a new scene loads (repeated
    // Restarts can compound this into real breakage - a hung scene load, not just Console
    // noise). The texture readback that goes wrong is driven by ARCameraManager, not ARSession
    // itself, so both need to stop before the scene unloads, and one frame isn't always enough
    // time for an in-flight GPU readback to finish or cancel - give it a few.
    IEnumerator LoadSceneAfterARSessionShutdown(string sceneName)
    {
        var cameraManager = FindFirstObjectByType<ARCameraManager>();
        if (cameraManager != null) cameraManager.enabled = false;

        var arSession = FindFirstObjectByType<ARSession>();
        if (arSession != null) arSession.enabled = false;

        if (cameraManager != null || arSession != null)
        {
            yield return null;
            yield return null;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
