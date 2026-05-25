using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterButton : MonoBehaviour {
    public Character Character;

    MainMenuUI _mainMenuUI;

    private void Awake() {
        _mainMenuUI = transform.root.GetComponent<MainMenuUI>();
    }

    public void OnClick() {
        _mainMenuUI.OpenCharacterEditing(Character);
        _mainMenuUI.EditingCharacter = Character;
    }
}
