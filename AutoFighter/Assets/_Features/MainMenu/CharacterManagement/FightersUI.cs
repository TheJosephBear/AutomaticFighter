using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightersUI : MonoBehaviour {

    public GameObject CharacterButtonPrefab;

    // UI refs
    public GameObject CharacterEditingView;
    public GameObject CharacterScrollViewContent;
    public RawImage FighterPreview;
    public TMP_InputField InputFieldCharacterName;
    public TextMeshProUGUI SkillPointsLabel; // Drag your UI Text for Skill Points here in Inspector
    public TextMeshProUGUI HPLabel;
    public TextMeshProUGUI DMGLabel;
    public TextMeshProUGUI ASLabel;
    public TextMeshProUGUI MSLabel;
    public TMP_Dropdown CharacterModelDropdown;

    [HideInInspector]
    public Character ActivelyEditedCharacter;

    ObjectUIPreviewManager _previewManager;

    void Awake() {
        CharacterEditingView.SetActive(false);
    }

    void Start() {
        _previewManager = FindAnyObjectByType<ObjectUIPreviewManager>();
        UpdateScrollView();
    }

    public void OnAddCharacter() {
        Character newCharacter = new Character {
            Name = "Jožan",
            HP = 100,
            DMG = 5,
            AS = 0.8f,
            MS = 10,
            CharacterModel = CharacterModel.Knight,
            SkillPoints = 5 // Initial skill points to spend
        };

        CharacterManager.Instance.AddCharacter(newCharacter);
        UpdateScrollView();
    }

    public void OnEditCharacter(Character character) {
        OpenCharacterEditing(character);
    }

    public void OnEditingFinished() {
        CharacterEditingView.SetActive(false);
        UpdateScrollView();
        CharacterManager.Instance.SaveCharacters();
    }

    public void OnRemoveCharacter() {
        CharacterManager.Instance.DeleteCharacter(ActivelyEditedCharacter);
        OnEditingFinished();
        CharacterManager.Instance.SaveCharacters();
    }

    public void OpenCharacterEditing(Character character) {
        ActivelyEditedCharacter = character;
        CharacterEditingView.SetActive(true);

        // Fill in data
        InputFieldCharacterName.text = character.Name;
        UpdateStatLabels();

        // Preview
        FighterPreview.texture = _previewManager.GetObjectPreviewTexture(((int)character.CharacterModel));

        // Dropdown
        RefreshCharacterModelDropdown();
        int targetIndex = CharacterManager.Instance.CharacterPrefabList.FindIndex(ch => ch.CharacterModelEnum == character.CharacterModel);
        CharacterModelDropdown.value = targetIndex >= 0 ? targetIndex : 0;
        CharacterModelDropdown.RefreshShownValue();
    }

    void RefreshCharacterModelDropdown() {
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();
        foreach (CharacterModelPairing ch in CharacterManager.Instance.CharacterPrefabList) {
            newOptions.Add(new TMP_Dropdown.OptionData(ch.Name));
        }

        CharacterModelDropdown.ClearOptions();
        CharacterModelDropdown.options = newOptions;
    }

    void UpdateScrollView() {
        SafelyDestroyAllItems();
        foreach (Character character in CharacterManager.Instance.CharacterList) {
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

        for (int i = contentRef.childCount - 1; i >= 0; i--) {
            GameObject child = contentRef.GetChild(i).gameObject;
            if (child != null) {
                Object.Destroy(child);
            }
        }

        if (scrollRect != null) {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        Canvas.ForceUpdateCanvases();

        if (contentRef.TryGetComponent<HorizontalOrVerticalLayoutGroup>(out var layoutGroup)) {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRef);
        }
    }

    private void UpdateStatLabels() {
        if (ActivelyEditedCharacter == null) return;

        if (SkillPointsLabel != null) {
            SkillPointsLabel.text = $"{ActivelyEditedCharacter.SkillPoints}";
        }
        HPLabel.text = ActivelyEditedCharacter.HP.ToString();
        DMGLabel.text = ActivelyEditedCharacter.DMG.ToString();
        ASLabel.text = ActivelyEditedCharacter.AS.ToString("F1");
        MSLabel.text = ActivelyEditedCharacter.MS.ToString();
    }

    #region Editing OnClicks

    public void OnCharacterModelChanged(int index) {
        ActivelyEditedCharacter.CharacterModel = CharacterManager.Instance.CharacterPrefabList[index].CharacterModelEnum;
        FighterPreview.texture = _previewManager.GetObjectPreviewTexture(((int)ActivelyEditedCharacter.CharacterModel));
    }

    public void SetCharacterName(string name) {
        ActivelyEditedCharacter.Name = name;
    }

    // --- HP ---
    public void OnCharacterHPAdd() {
        if (ActivelyEditedCharacter.SkillPoints <= 0) return;

        ActivelyEditedCharacter.HP += 50;
        ActivelyEditedCharacter.SkillPoints--;
        UpdateStatLabels();
    }

    public void OnCharacterHPDecrease() {
        if (ActivelyEditedCharacter.HP > 50) {
            ActivelyEditedCharacter.HP -= 50;
            ActivelyEditedCharacter.SkillPoints++;
            UpdateStatLabels();
        }
    }

    // --- DMG ---
    public void OnCharacterDMGAdd() {
        if (ActivelyEditedCharacter.SkillPoints <= 0) return;

        ActivelyEditedCharacter.DMG += 2;
        ActivelyEditedCharacter.SkillPoints--;
        UpdateStatLabels();
    }

    public void OnCharacterDMGDecrease() {
        if (ActivelyEditedCharacter.DMG > 5) {
            ActivelyEditedCharacter.DMG -= 2;
            ActivelyEditedCharacter.SkillPoints++;
            UpdateStatLabels();
        }
    }

    // --- Attack Speed (AS) - Smaller value means faster attack speed ---
    public void OnCharacterASAdd() {
        if (ActivelyEditedCharacter.SkillPoints <= 0 || ActivelyEditedCharacter.AS <= 0.1f) return;

        ActivelyEditedCharacter.AS -= 0.1f; // Decreasing delay costs 1 point
        ActivelyEditedCharacter.SkillPoints--;
        UpdateStatLabels();
    }

    public void OnCharacterASDecrease() {
        if (ActivelyEditedCharacter.AS < 1.5f) {
            ActivelyEditedCharacter.AS += 0.1f; // Increasing delay refunds 1 point
            ActivelyEditedCharacter.SkillPoints++;
            UpdateStatLabels();
        }
    }

    // --- Movement Speed (MS) ---
    public void OnCharacterMSAdd() {
        if (ActivelyEditedCharacter.SkillPoints <= 0) return;

        ActivelyEditedCharacter.MS++;
        ActivelyEditedCharacter.SkillPoints--;
        UpdateStatLabels();
    }

    public void OnCharacterMSDecrease() {
        if (ActivelyEditedCharacter.MS > 2) {
            ActivelyEditedCharacter.MS--;
            ActivelyEditedCharacter.SkillPoints++;
            UpdateStatLabels();
        }
    }

    #endregion
}