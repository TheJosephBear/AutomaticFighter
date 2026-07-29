using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour {

    public GameObject ItemPrefab;
    public GameObject ContentRef;

    List<LeaderboardItem> _itemList = new List<LeaderboardItem>();

    void OnEnable() {
        // Automatically refresh whenever the Leaderboard UI is enabled/shown
        UpdateLeaderboard();
    }

    public void UpdateLeaderboard() {
        SafelyDestroyAllItems();

        if (PlayerManager.Instance == null || PlayerManager.Instance.PlayerList == null) return;

        // 1. Order players descending by Points (and by WinCount as a tiebreaker)
        List<Player> sortedPlayers = PlayerManager.Instance.PlayerList
            .OrderBy(p => p.Points)
            .ToList();

        // 2. Add sorted items to UI with 1-based rank position
        for (int i = 0; i < sortedPlayers.Count; i++) {
            Player p = sortedPlayers[i];
            AddItem(p.Name, i + 1, p.Points);
        }
    }

    public void AddItem(string name, int order, int points) {
        LeaderboardItem item = Instantiate(ItemPrefab, ContentRef.transform).GetComponent<LeaderboardItem>();
        item.SetName(name);
        item.SetOrder(order.ToString());
        item.SetPoints(points.ToString());
        _itemList.Add(item);
    }

    public void Clear() {
        _itemList.Clear();
        SafelyDestroyAllItems();
    }

    void SafelyDestroyAllItems() {
        if (ContentRef == null) {
            Debug.LogWarning("ContentRef Reference is missing!");
            return;
        }

        RectTransform contentRef = ContentRef.GetComponent<RectTransform>();
        ScrollRect scrollRect = contentRef.GetComponent<ScrollRect>();

        // 1. Loop backwards to safely destroy all children
        for (int i = contentRef.childCount - 1; i >= 0; i--) {
            GameObject child = contentRef.GetChild(i).gameObject;

            // Safety check to ensure we aren't destroying something already dead
            if (child != null) {
                Object.Destroy(child);
            }
        }

        // 2. Reset the scroll position to the top of the view
        if (scrollRect != null) {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        // 3. Force Unity's UI system to instantly recalculate the sizes
        Canvas.ForceUpdateCanvases();

        if (contentRef.TryGetComponent<HorizontalOrVerticalLayoutGroup>(out var layoutGroup)) {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRef);
        }
    }

}
