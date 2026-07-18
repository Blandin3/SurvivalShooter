using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    public Transform listParent;
    public GameObject entryRowPrefab;

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in listParent)
        {
            Destroy(child.gameObject);
        }

        if (LeaderboardManager.Instance == null) return;

        var entries = LeaderboardManager.Instance.GetEntries();
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            GameObject row = Instantiate(entryRowPrefab, listParent);
            var text = row.GetComponent<TMP_Text>();
            if (text != null)
            {
                text.color = Color.yellow;
                text.text = string.Format("{0}. {1}   Score: {2}   Kills: {3}   Time: {4}s",
                    i + 1, entry.date, entry.score, entry.enemiesDefeated, Mathf.CeilToInt(entry.timeSurvived));
            }
        }
    }
}
