using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListUI : MonoBehaviour {

    public GameObject PlayerEditingView;
    public GameObject PlayerItemPrefab;
    public GameObject ContentRef;

    PlayerEditingUI _playerEditingUI;
    PlayerManager _playerManager;

    void Start() {
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _playerEditingUI = PlayerEditingView.GetComponent<PlayerEditingUI>();

        UpdateList();
    }

    public void OnAddPlayer() {
        Player player = _playerManager.AddNewPlayer();
        OnEditPlayer(player);
    }

    public void OnEditPlayer(Player player) {
        ToggleEditingUI(true);
        _playerEditingUI.InitializeUI(player, this);
    }

    public void OnRemovePlayer(Player player) {
        _playerManager.RemovePlayer(player);
        UpdateList();
    }

    public void ToggleEditingUI(bool toggleOn) {
        PlayerEditingView.SetActive(toggleOn);
    }

    public void UpdateList() {
        ClearListUI();
        foreach (Player player in _playerManager.PlayerList) {
            PlayerInfoItem item = Instantiate(
                PlayerItemPrefab,
                ContentRef.transform
            ).GetComponent<PlayerInfoItem>();

            item.InitializeUI(player, this);
        }
    }

    public void ClearListUI() {
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
