using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    public GameObject MainView;
    public GameObject PlayerView;
    public GameObject FighterView;
    public LeaderboardUI LeaderBoardRef;

    PlayerManager _playerManager;
    ObjectUIPreviewManager _previewManager;
    CharacterManager _characterManager;

    void Awake() {
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _previewManager = FindAnyObjectByType<ObjectUIPreviewManager>();
        _characterManager = FindAnyObjectByType<CharacterManager>();
        //    CloseCharacterEditing();

        //     StartCoroutine(WaitForInitalization());
        OpenMainMenu();
    }

    public void StartFight() {

    }

    public void OpenMainMenu() {
        MainView.SetActive(true);
        PlayerView.SetActive(false);
        FighterView.SetActive(false);
        UpdateLeaderboard();
    }

    public void OpenFighterView() {
        MainView.SetActive(false);
        PlayerView.SetActive(false);
        FighterView.SetActive(true);
    }

    public void OpenPlayerView() {
        MainView.SetActive(false);
        PlayerView.SetActive(true);
        FighterView.SetActive(false);
    }

    void UpdateLeaderboard() {
        LeaderBoardRef.Clear();
        if (_playerManager.PlayerList.Count < 1) return;

        List<Player> sortedList = _playerManager.PlayerList.OrderBy(player => player.Points).ToList();
        for (int i = 0; i < sortedList.Count; i++) {
            Player player = sortedList[i];
            LeaderBoardRef.AddItem(player.Name, i+1, player.Points);
        }
    }
}