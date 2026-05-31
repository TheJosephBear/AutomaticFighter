using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class FightersUI : MonoBehaviour {

    public GameObject CharacterButtonPrefab;

    // UI reffs
    public GameObject CharacterEditingView;
    public GameObject CharacterScrollViewContent;
    public RawImage FighterPreview;
    public TMP_InputField InputFieldCharacterName;
    public TextMeshProUGUI HPLabel;
    public TextMeshProUGUI DMGLabel;
    public TextMeshProUGUI ASLabel;
    public TextMeshProUGUI MSLabel;
    public TMP_Dropdown CharacterModelDropdown;

    [HideInInspector]
    public Character ActivelyEditedCharacter;

    CharacterManager _characterManager;
    ObjectUIPreviewManager _previewManager;

    void Awake() {
        _characterManager = FindAnyObjectByType<CharacterManager>();
        _previewManager = FindAnyObjectByType<ObjectUIPreviewManager>();

        CharacterEditingView.SetActive(false);
    }

    public void OnAddCharacter() {
        Character newCharacter = new Character {
            Name = "Jožan",
            HP = 100,
            DMG = 5,
            AS = 0.8f,
            MS = 10,
            CharacterModel = CharacterModel.Knight,
        };

        _characterManager.AddCharacter(newCharacter);
        UpdateScrollView();
    }

    public void OnEditCharacter(Character character) {
        OpenCharacterEditing(character);
    }

    public void OnEditingFinished() {
        CharacterEditingView.SetActive(false);
        UpdateScrollView();
    }

    public void OnRemoveCharacter() {
        _characterManager.DeleteCharacter(ActivelyEditedCharacter);
        OnEditingFinished();
    }

    public void OpenCharacterEditing(Character character) {
        ActivelyEditedCharacter = character;
        CharacterEditingView.SetActive(true);
        // Fill in data
        InputFieldCharacterName.text = character.Name;
        HPLabel.text = character.HP.ToString();
        DMGLabel.text = character.DMG.ToString();
        ASLabel.text = character.AS.ToString();
        MSLabel.text = character.MS.ToString();

        // Preview
        FighterPreview.texture = _previewManager.GetObjectPreviewTexture(((int)character.CharacterModel));

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

    void UpdateScrollView() {
        SafelyDestroyAllItems();
        foreach (Character character in _characterManager.CharacterList) {
            CharacterButton button = Instantiate(CharacterButtonPrefab, CharacterScrollViewContent.transform).GetComponent<CharacterButton>();
            button.Character = character;
            button.FightersUIReff = this;
            button.SetNameText(character.Name);
        }
    }

    void SafelyDestroyAllItems() {
        if (CharacterScrollViewContent == null) {
            Debug.LogWarning("ContentRef Reference is missing!");
            return;
        }

        RectTransform contentRef = CharacterScrollViewContent.GetComponent<RectTransform>();
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

    #region Editing OnClicks

    public void OnCharacterModelChanged(int index) {
        ActivelyEditedCharacter.CharacterModel = _characterManager.CharacterPrefabList[index].CharacterModelEnum;
        FighterPreview.texture = _previewManager.GetObjectPreviewTexture(((int)ActivelyEditedCharacter.CharacterModel));
    }

    public void SetCharacterName(string name) {
        ActivelyEditedCharacter.Name = name;
    }

    public void OnCharacterHPAdd() {
        ActivelyEditedCharacter.HP++;
        HPLabel.text = ActivelyEditedCharacter.HP.ToString();
    }

    public void OnCharacterHPDecrease() {
        ActivelyEditedCharacter.HP--;
        HPLabel.text = ActivelyEditedCharacter.HP.ToString();
    }

    public void OnCharacterDMGAdd() {
        ActivelyEditedCharacter.DMG++;
        DMGLabel.text = ActivelyEditedCharacter.DMG.ToString();
    }

    public void OnCharacterDMGDecrease() {
        ActivelyEditedCharacter.DMG--;
        DMGLabel.text = ActivelyEditedCharacter.DMG.ToString();
    }

    public void OnCharacterASAdd() {
        ActivelyEditedCharacter.AS++;
        ASLabel.text = ActivelyEditedCharacter.AS.ToString();
    }

    public void OnCharacterASDecrease() {
        ActivelyEditedCharacter.AS--;
        ASLabel.text = ActivelyEditedCharacter.AS.ToString();
    }

    public void OnCharacterMSAdd() {
        ActivelyEditedCharacter.MS++;
        MSLabel.text = ActivelyEditedCharacter.MS.ToString();
    }

    public void OnCharacterMSDecrease() {
        ActivelyEditedCharacter.MS--;
        MSLabel.text = ActivelyEditedCharacter.MS.ToString();
    }

    #endregion

}