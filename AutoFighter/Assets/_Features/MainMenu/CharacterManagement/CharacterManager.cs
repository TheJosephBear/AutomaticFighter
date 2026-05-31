using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {
    
    public List<Character> CharacterList = new List<Character>();
    public List<CharacterModelPairing> CharacterPrefabList = new List<CharacterModelPairing>();
    [HideInInspector]
    public Character SelectedCharacter1;
    [HideInInspector]
    public Character SelectedCharacter2;

    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void AddCharacter(
        float HP,
        float DMG,
        float AS,
        float MS,
        CharacterModel CharacterModel) {
        CharacterList.Add(new Character {
            HP = HP, 
            DMG = DMG, 
            AS = AS, 
            MS = MS, 
            CharacterModel = CharacterModel
        });
    }

    public void AddCharacter(Character character) {
        CharacterList.Add(character);
    }

    public void EditCharacter(
        int index,
        float HP,
        float DMG,
        float AS,
        float MS,
        CharacterModel CharacterModel
    ) {
        CharacterList[index].HP = HP;
        CharacterList[index].DMG = DMG;
        CharacterList[index].AS = AS;
        CharacterList[index].MS = MS;
        CharacterList[index].CharacterModel = CharacterModel;
    }

    public void SelectCharacter1(int index) {
        SelectedCharacter1 = CharacterList[index];
    }

    public void SelectCharacter2(int index) {
        SelectedCharacter2 = CharacterList[index];
    }

    public void DeleteCharacter(Character character) {
        CharacterList.Remove(character);
    }

}

public class Character {
    public string Name;
    public float HP;
    public float DMG;
    public float AS;
    public float MS;
    public CharacterModel CharacterModel;
}

public enum CharacterModel {
    Knight,
    Freddy
}

[Serializable]
public class CharacterModelPairing {
    public string Name;
    public string Description;
    public CharacterModel CharacterModelEnum;
    public GameObject CharacterEntityPrefab;
}