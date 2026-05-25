using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerInfoItem : MonoBehaviour {

    public TextMeshProUGUI NameLabel;
    public TextMeshProUGUI PointCountLabel;

    PlayerListUI _playerListUI;
    Player _player;

    void Start() {

    }

    public void InitializeUI(Player player, PlayerListUI playerListUI) {
        _playerListUI = playerListUI;
        _player = player;

        NameLabel.text = player.Name;
        PointCountLabel.text = player.Points.ToString();
    }

    public void OnEdit() {
        _playerListUI.OnEditPlayer(_player);
    }

}
