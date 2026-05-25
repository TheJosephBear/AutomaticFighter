using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public List<Player> PlayerList = new List<Player>();

    void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public Player AddNewPlayer() {
        Player player = new Player();
        player.Points = 3;
        PlayerList.Add(player);

        return player;
    }

    public void SetPlayerName(Player player, string newName) {
        player.Name = newName;
    }

    public void AddPlayerPoint(Player player, int pointNumber) {
        player.Points = player.Points + pointNumber;
    }

    public void AddPlayerWin(Player player) {
        player.WinCount = player.WinCount + 1;
    }

    public void RemovePlayer(Player player) {
        PlayerList.Remove(player);
    }

}

public class Player {
    public string Name;
    public int Points;
    public int WinCount;
}