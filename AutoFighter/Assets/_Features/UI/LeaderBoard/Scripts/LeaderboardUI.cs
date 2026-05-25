using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour {

    public GameObject ItemPrefab;
    public GameObject ContentRef;

    List<LeaderboardItem> _itemList = new List<LeaderboardItem>();

    void Awake() {
        
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
