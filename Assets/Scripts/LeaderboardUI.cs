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

        foreach (var entry in LeaderboardManager.Instance.GetEntries())
        {
            GameObject row = Instantiate(entryRowPrefab, listParent);
            var text = row.GetComponent<TMP_Text>();
            if (text != null)
            {
                text.text = string.Format("{0}   Score: {1}   Kills: {2}   Time: {3}s",
                    entry.date, entry.score, entry.enemiesDefeated, Mathf.CeilToInt(entry.timeSurvived));
            }
        }
    }
}
