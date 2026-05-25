using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerEditingUI : MonoBehaviour {

    Player _player;
    PlayerListUI _listUI;

    public TMP_InputField NameInput;

    void Start() {

    }

    public void InitializeUI(Player player, PlayerListUI listUI) {
        _player = player;
        _listUI = listUI;

        NameInput.text = player.Name;
    }

    public void OnNameChanged(string name) {
        _player.Name = name;
    }

    public void OnFinished() {
        _listUI.UpdateList();
        _listUI.ToggleEditingUI(false);
    }

    public void DeletePlayer() {
        _listUI.OnRemovePlayer(_player);
        _listUI.ToggleEditingUI(false);
    }
}
