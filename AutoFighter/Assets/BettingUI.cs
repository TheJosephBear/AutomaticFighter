using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BettingUI : MonoBehaviour {

    // UI References
    public TMP_Dropdown CharacterSelect1Dropdown;
    public TMP_Dropdown CharacterSelect2Dropdown;
    public RawImage Fighter1Preview;
    public RawImage Fighter2Preview;

    public GameObject BettingPlayerPrefab;
    public GameObject BettingScrollViewContent;

    // Internal trackers
    private CharacterManager _characterManager;
    private PlayerManager _playerManager;
    private ObjectUIPreviewManager _previewManager;

    // Track what players have bet on (Player instance -> Choice/Bet amount mapping)
    // You can track this in a custom runtime class or inside the Player object if preferred
    private Dictionary<Player, int> playerBets = new Dictionary<Player, int>();
    private Dictionary<Player, int> playerChoices = new Dictionary<Player, int>(); // 1 for Fighter1, 2 for Fighter2

    void Awake() {
        _characterManager = FindAnyObjectByType<CharacterManager>();
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _previewManager = FindAnyObjectByType<ObjectUIPreviewManager>();
    }

    void OnEnable() {
        UpdateBettingView();
    }

    void UpdateBettingView() {
        InitializeDropdowns();
        UpdateBetterList();
    }

    void InitializeDropdowns() {
        CharacterSelect1Dropdown.ClearOptions();
        CharacterSelect2Dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (Character character in _characterManager.CharacterList) {
            options.Add(new TMP_Dropdown.OptionData(character.Name));
        }

        CharacterSelect1Dropdown.AddOptions(options);
        CharacterSelect2Dropdown.AddOptions(options);

        // Select defaults if they exist
        if (_characterManager.CharacterList.Count > 0) {
            OnCharacterSelected1(0);
            if (_characterManager.CharacterList.Count > 1) {
                CharacterSelect2Dropdown.value = 1;
                OnCharacterSelected2(1);
            } else {
                OnCharacterSelected2(0);
            }
        }
    }

    void UpdateBetterList() {
        SafelyDestroyAllItems();

        foreach (Player player in _playerManager.PlayerList) {
            BettingPlayerItem item = Instantiate(BettingPlayerPrefab, BettingScrollViewContent.transform)
                                     .GetComponent<BettingPlayerItem>();
            item.InitializeUI(player, this);
        }
    }

    #region Betting OnClicks

    public void OnCharacterSelected1(int index) {
        if (_characterManager.CharacterList.Count == 0) return;
        _characterManager.SelectCharacter1(index);
        Fighter1Preview.texture = _previewManager.GetObjectPreviewTexture((int)_characterManager.SelectedCharacter1.CharacterModel);
    }

    public void OnCharacterSelected2(int index) {
        if (_characterManager.CharacterList.Count == 0) return;
        _characterManager.SelectCharacter2(index);
        Fighter2Preview.texture = _previewManager.GetObjectPreviewTexture((int)_characterManager.SelectedCharacter2.CharacterModel);
    }

    public void BetOnFighter1(Player player, int amount) {
        playerChoices[player] = 1;
        playerBets[player] = amount;
    }

    public void BetOnFighter2(Player player, int amount) {
        playerChoices[player] = 2;
        playerBets[player] = amount;
    }

    public void OnGAMBA(Player player, int finalizedBet) {
        if (!playerChoices.ContainsKey(player)) {
            Debug.LogWarning($"{player.Name} hasn't selected a fighter to bet on!");
            return;
        }

        int choice = playerChoices[player];

        // Deduct points instantly or lock them for the match
        _playerManager.AddPlayerPoint(player, -finalizedBet);

        Debug.Log($"{player.Name} gambled {finalizedBet} points on Fighter {choice}!");

        // Refresh the list UI to show updated available points
        UpdateBetterList();
    }

    #endregion

    void SafelyDestroyAllItems() {
        if (BettingScrollViewContent == null) return;
        for (int i = BettingScrollViewContent.transform.childCount - 1; i >= 0; i--) {
            Destroy(BettingScrollViewContent.transform.GetChild(i).gameObject);
        }
        Canvas.ForceUpdateCanvases();
    }
}