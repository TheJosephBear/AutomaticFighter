using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BettingUI : MonoBehaviour {

    public TMP_Dropdown CharacterSelect1Dropdown;
    public TMP_Dropdown CharacterSelect2Dropdown;
    public RawImage Fighter1Preview;
    public RawImage Fighter2Preview;

    public GameObject BettingPlayerPrefab;
    public GameObject BettingScrollViewContent;

    ObjectUIPreviewManager _previewManager;

    private List<BettingPlayerItem> _activeItems = new List<BettingPlayerItem>();

    void Start() { 
        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine() {
        yield return new WaitForSeconds(0.2f);
        UpdateBettingView();
    }

    public void UpdateBettingView() {
        _previewManager = FindAnyObjectByType<ObjectUIPreviewManager>();

        InitializeDropdowns();
        UpdateBetterList();
    }

    void InitializeDropdowns() {
        CharacterSelect1Dropdown.ClearOptions();
        CharacterSelect2Dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (Character character in CharacterManager.Instance.CharacterList) {
            options.Add(new TMP_Dropdown.OptionData(character.Name));
        }

        CharacterSelect1Dropdown.AddOptions(options);
        CharacterSelect2Dropdown.AddOptions(options);

        if (CharacterManager.Instance.CharacterList.Count > 0) {
            OnCharacterSelected1(0);
            int secondChoice = CharacterManager.Instance.CharacterList.Count > 1 ? 1 : 0;
            CharacterSelect2Dropdown.value = secondChoice;
            OnCharacterSelected2(secondChoice);
        }
    }

    public void UpdateBetterList() {
        SafelyDestroyAllItems();
        _activeItems.Clear();


        foreach (Player player in PlayerManager.Instance.PlayerList) {
            BettingPlayerItem item = Instantiate(BettingPlayerPrefab, BettingScrollViewContent.transform)
                                     .GetComponent<BettingPlayerItem>();
            item.InitializeUI(player, this);
            _activeItems.Add(item);
        }

        RefreshFighterNamesOnItems();
    }

    public void OnCharacterSelected1(int index) {
        if (CharacterManager.Instance.CharacterList.Count == 0) return;
        CharacterManager.Instance.SelectCharacter1(index);
        Fighter1Preview.texture = _previewManager.GetObjectPreviewTexture((int)CharacterManager.Instance.SelectedCharacter1.CharacterModel);
        RefreshFighterNamesOnItems();
    //    UpdateBetterList();
    }

    public void OnCharacterSelected2(int index) {
        if (CharacterManager.Instance.CharacterList.Count == 0) return;
        CharacterManager.Instance.SelectCharacter2(index);
        Fighter2Preview.texture = _previewManager.GetObjectPreviewTexture((int)CharacterManager.Instance.SelectedCharacter2.CharacterModel);
        RefreshFighterNamesOnItems();
    //    UpdateBetterList();
    }

    private void RefreshFighterNamesOnItems() {
        string f1Name = CharacterManager.Instance.SelectedCharacter1 != null ? CharacterManager.Instance.SelectedCharacter1.Name : "Fighter 1";
        string f2Name = CharacterManager.Instance.SelectedCharacter2 != null ? CharacterManager.Instance.SelectedCharacter2.Name : "Fighter 2";

        foreach (var item in _activeItems) {
            if (item != null) item.SetFighterNames(f1Name, f2Name);
        }
    }

    void SafelyDestroyAllItems() {
        if (BettingScrollViewContent == null) return;
        for (int i = BettingScrollViewContent.transform.childCount - 1; i >= 0; i--) {
            Destroy(BettingScrollViewContent.transform.GetChild(i).gameObject);
        }
        Canvas.ForceUpdateCanvases();
    }
}