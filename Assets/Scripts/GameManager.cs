using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; } = GameState.MainMenu;

    public float matchDuration = 120f;
    public float TimeRemaining { get; private set; }
    public int Score { get; private set; }
    public int EnemiesDefeated { get; private set; }

    [Tooltip("Scene that should auto-start gameplay each time it loads (including reloads via Restart), until real AR-placement-triggered StartGame() replaces this.")]
    public string gameSceneName = "GameScene";

    public event Action<int> OnScoreChanged;
    public event Action<float> OnTimeChanged;
    public event Action OnGameStarted;
    public event Action OnGameEnded;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("GameManager.Awake(): a GameManager.Instance already existed, destroying this duplicate.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameManager.Awake(): Instance set, initial State is " + State, this);

        // GameManager persists across scene reloads (DontDestroyOnLoad), so Start() on a
        // scene-local object only ever fires once - it can't be used to detect a repeated
        // Restart into the same scene. SceneManager.sceneLoaded fires every single time a
        // scene finishes loading instead, including reloads of the same scene, which is what
        // repeated Restarts need.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameSceneName && State != GameState.Playing)
        {
            Debug.Log("GameManager.OnSceneLoaded(): '" + scene.name + "' loaded, starting game.", this);
            StartGame();
        }
    }

    void Update()
    {
        if (State != GameState.Playing) return;

        TimeRemaining -= Time.deltaTime;
        OnTimeChanged?.Invoke(TimeRemaining);

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            EndGame();
        }
    }

    public void StartPlacement()
    {
        State = GameState.Placement;
    }

    [ContextMenu("Test Start Game")]
    public void StartGame()
    {
        Score = 0;
        EnemiesDefeated = 0;
        TimeRemaining = matchDuration;
        State = GameState.Playing;

        Debug.Log("GameManager.StartGame(): State is now Playing. OnGameStarted has " + (OnGameStarted?.GetInvocationList().Length ?? 0) + " listener(s).", this);

        OnGameStarted?.Invoke();
        OnScoreChanged?.Invoke(Score);
        OnTimeChanged?.Invoke(TimeRemaining);
    }

    public void RegisterKill(int points)
    {
        if (State != GameState.Playing) return;

        Score += points;
        EnemiesDefeated++;
        OnScoreChanged?.Invoke(Score);
    }

    public void EndGame()
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;

        foreach (var enemy in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
        {
            enemy.Wipe();
        }

        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.SubmitSession(Score, EnemiesDefeated, matchDuration - TimeRemaining);
        }

        Debug.Log("GameManager.EndGame(): State is now GameOver. OnGameEnded has " + (OnGameEnded?.GetInvocationList().Length ?? 0) + " listener(s).", this);

        OnGameEnded?.Invoke();
    }

    public void ReturnToMainMenu()
    {
        State = GameState.MainMenu;
    }
}
