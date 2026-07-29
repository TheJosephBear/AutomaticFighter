using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager> {

    private const string SAVE_KEY = "Saved_Characters_Data";

    public List<Character> CharacterList = new List<Character>();
    public List<CharacterModelPairing> CharacterPrefabList = new List<CharacterModelPairing>();

    [HideInInspector]
    public Character SelectedCharacter1;
    [HideInInspector]
    public Character SelectedCharacter2;

    protected override void Awake() {
        base.Awake();
        LoadCharacters();
    }

    public void AddCharacter(float HP, float DMG, float AS, float MS, CharacterModel model, int skillPoints = 5) {
        Character character = new Character {
            Name = "Fighter " + (CharacterList.Count + 1),
            HP = HP,
            DMG = DMG,
            AS = AS,
            MS = MS,
            CharacterModel = model,
            SkillPoints = skillPoints
        };
        AddCharacter(character);
    }

    public void AddCharacter(Character character) {
        CharacterList.Add(character);
        SaveCharacters();
    }

    public void EditCharacter(int index, string name, float HP, float DMG, float AS, float MS, CharacterModel model, int skillPoints) {
        if (index < 0 || index >= CharacterList.Count) return;

        CharacterList[index].Name = name;
        CharacterList[index].HP = HP;
        CharacterList[index].DMG = DMG;
        CharacterList[index].AS = AS;
        CharacterList[index].MS = MS;
        CharacterList[index].CharacterModel = model;
        CharacterList[index].SkillPoints = skillPoints;

        SaveCharacters();
    }

    public void SelectCharacter1(int index) {
        if (index >= 0 && index < CharacterList.Count) {
            SelectedCharacter1 = CharacterList[index];
        }
    }

    public void SelectCharacter2(int index) {
        if (index >= 0 && index < CharacterList.Count) {
            SelectedCharacter2 = CharacterList[index];
        }
    }

    public void DeleteCharacter(Character character) {
        if (SelectedCharacter1 == character) SelectedCharacter1 = null;
        if (SelectedCharacter2 == character) SelectedCharacter2 = null;

        CharacterList.Remove(character);
        SaveCharacters();
    }

    public void SaveCharacters() {
        string json = JsonListHelper.ToJson(CharacterList);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save(); // Works cross-platform, including WebGL browser storage
    }

    public void LoadCharacters() {
        if (PlayerPrefs.HasKey(SAVE_KEY)) {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            CharacterList = JsonListHelper.FromJson<Character>(json);
        }
    }

    public void ResetAllCharacterData() {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        CharacterList.Clear();
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class Character {
    public string Name;
    public float HP;
    public float DMG;
    public float AS;
    public float MS;
    public int SkillPoints = 5;
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