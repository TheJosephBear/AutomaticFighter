using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    public GameObject MainView;
    public GameObject PlayerView;
    public GameObject FighterView;
    public LeaderboardUI LeaderBoardRef;

    PlayerManager _playerManager;


    public GameObject CharacterButtonPrefab;

    public GameObject FightersScrollViewContent;
    public RawImage Fighter1Preview;
    public RawImage Fighter2Preview;
    public TMP_Dropdown CharacterSelect1Dropdown;
    public TMP_Dropdown CharacterSelect2Dropdown;

    public GameObject CharacterCreationView;
    public TextMeshProUGUI HPLabel;
    public TextMeshProUGUI DMGLabel;
    public TextMeshProUGUI ASLabel;
    public TextMeshProUGUI MSLabel;
    public TMP_Dropdown CharacterModelDropdown;

    [HideInInspector]
    public Character EditingCharacter;

    ObjectUIPreviewManager _previewManager;
    CharacterManager _characterManager;

    void Awake() {
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _previewManager = FindAnyObjectByType<ObjectUIPreviewManager>();
        _characterManager = FindAnyObjectByType<CharacterManager>();
        //    CloseCharacterEditing();

        //     StartCoroutine(WaitForInitalization());
        OpenMainMenu();
    }

    public void StartFight() {

    }

    public void OpenMainMenu() {
        MainView.SetActive(true);
        PlayerView.SetActive(false);
        FighterView.SetActive(false);
        UpdateLeaderboard();
    }

    public void OpenFighterView() {
        MainView.SetActive(false);
        PlayerView.SetActive(false);
        FighterView.SetActive(true);
    }

    public void OpenPlayerView() {
        MainView.SetActive(false);
        PlayerView.SetActive(true);
        FighterView.SetActive(false);
    }

    void UpdateLeaderboard() {
        LeaderBoardRef.Clear();
        if (_playerManager.PlayerList.Count < 1) return;

        List<Player> sortedList = _playerManager.PlayerList.OrderBy(player => player.Points).ToList();
        for (int i = 0; i < sortedList.Count; i++) {
            Player player = sortedList[i];
            LeaderBoardRef.AddItem(player.Name, i+1, player.Points);
        }
    }



    IEnumerator WaitForInitalization() {
        yield return new WaitForSeconds(0.2f);

        Fighter1Preview.texture = _previewManager.GetObjectPreviewTexture(0);
        Fighter2Preview.texture = _previewManager.GetObjectPreviewTexture(1);
    }

    void InitializeDropdowns() {
        CharacterSelect1Dropdown.options.Clear();
    }

    public void OpenCharacterEditing(Character character) {
        CharacterCreationView.SetActive(true);
        // Fill in data
        HPLabel.text = character.HP.ToString();
        DMGLabel.text = character.DMG.ToString();
        ASLabel.text = character.AS.ToString();
        MSLabel.text = character.MS.ToString();

        // Dropdown
        RefreshCharacterModelDropdown();
        int targetIndex = _characterManager.CharacterPrefabList.FindIndex(ch => ch.CharacterModelEnum == character.CharacterModel);
        CharacterModelDropdown.value = targetIndex >= 0 ? targetIndex : 0;
        CharacterModelDropdown.RefreshShownValue();
    }

    void RefreshCharacterModelDropdown() {
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();
        foreach (CharacterModelPairing ch in _characterManager.CharacterPrefabList) {
            newOptions.Add(new TMP_Dropdown.OptionData(ch.Name));
        }

        CharacterModelDropdown.ClearOptions();

        CharacterModelDropdown.options = newOptions;
    }

    public void CloseCharacterEditing() {
    //    CharacterCreationView.SetActive(false);
    }

    // Dropdown
    public void OnCharacterSelected1(int index) {
        _characterManager.SelectCharacter1(index);
    }

    // Dropdown
    public void OnCharacterSelected2(int index) {
        _characterManager.SelectCharacter2(index);
    }

    // Character creation/editing
    public void OnAddCharacter() {
        CharacterButton button = Instantiate(CharacterButtonPrefab, FightersScrollViewContent.transform).GetComponent<CharacterButton>();
        button.Character = new Character {
            HP = 100,
            DMG = 5,
            AS = 0.8f,
            MS = 10,
            CharacterModel = CharacterModel.Knight,
        };
    }

    public void OnCharacterModelChanged(int index) {
        EditingCharacter.CharacterModel = _characterManager.CharacterPrefabList[index].CharacterModelEnum;
    }

    public void OnCharacterHPAdd() {
        EditingCharacter.HP++;
    }

    public void OnCharacterHPDecrease() {
        EditingCharacter.HP--;

    }

    public void OnCharacterDMGAdd() {
        EditingCharacter.DMG++;

    }

    public void OnCharacterDMGDecrease() {
        EditingCharacter.DMG--;

    }

    public void OnCharacterASAdd() {
        EditingCharacter.AS++;

    }

    public void OnCharacterASDecrease() {
        EditingCharacter.AS--;

    }

    public void OnCharacterMSAdd() {
        EditingCharacter.MS++;

    }

    public void OnCharacterMSDecrease() {
        EditingCharacter.MS--;

    }





}
