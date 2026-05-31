using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterButton : MonoBehaviour {

    public TextMeshProUGUI NameTextRef;

    public Character Character;
    public FightersUI FightersUIReff;

    MainMenuUI _mainMenuUI;

    private void Awake() {
        _mainMenuUI = transform.root.GetComponent<MainMenuUI>();
    }

    public void OnEdit() {
        FightersUIReff.OnEditCharacter(Character);
    }

    public void SetNameText(string name) {
        NameTextRef.text = name;
    }
}
