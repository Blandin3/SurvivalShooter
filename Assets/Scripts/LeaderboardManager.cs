using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    const string PrefsKey = "SurvivalShooter_Leaderboard";
    const int MaxEntries = 5;

    LeaderboardData data;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    void Load()
    {
        string json = PlayerPrefs.GetString(PrefsKey, "");
        data = string.IsNullOrEmpty(json) ? new LeaderboardData() : JsonUtility.FromJson<LeaderboardData>(json);
    }

    void Save()
    {
        PlayerPrefs.SetString(PrefsKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public void SubmitSession(int score, int enemiesDefeated, float timeSurvived)
    {
        data.entries.Insert(0, new LeaderboardEntry
        {
            score = score,
            enemiesDefeated = enemiesDefeated,
            timeSurvived = timeSurvived,
            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        });

        if (data.entries.Count > MaxEntries)
        {
            data.entries.RemoveRange(MaxEntries, data.entries.Count - MaxEntries);
        }

        Save();
    }

    public List<LeaderboardEntry> GetEntries()
    {
        return data.entries;
    }
}
