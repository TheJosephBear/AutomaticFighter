using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager> {

    private const string SAVE_KEY = "Saved_Players_Data";

    public List<Player> PlayerList = new List<Player>();

    protected override void Awake() {
        base.Awake();
        LoadPlayers();
    }

    public Player AddNewPlayer() {
        Player player = new Player {
            Name = "Player " + (PlayerList.Count + 1),
            Points = 3,
            WinCount = 0
        };
        PlayerList.Add(player);
        SavePlayers();
        return player;
    }

    public void SetPlayerName(Player player, string newName) {
        player.Name = newName;
        SavePlayers();
    }

    public void AddPlayerPoint(Player player, int pointNumber) {
        player.Points += pointNumber;
        if (player.Points <= 0) player.Points = 1;
        SavePlayers();
    }

    public void AddPlayerWin(Player player) {
        player.WinCount += 1;
        SavePlayers();
    }

    public void RemovePlayer(Player player) {
        PlayerList.Remove(player);
        SavePlayers();
    }

    public void SavePlayers() {
        string json = JsonListHelper.ToJson(PlayerList);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save(); // Flushes to IndexedDB in WebGL
    }

    public void LoadPlayers() {
        if (PlayerPrefs.HasKey(SAVE_KEY)) {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            PlayerList = JsonListHelper.FromJson<Player>(json);
        }
    }

    public void ResetAllPlayerData() {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerList.Clear();
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class Player {
    public string Name;
    public int Points;
    public int WinCount;
}