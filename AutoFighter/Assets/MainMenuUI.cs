using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    public string MainMenuSceneName = "";
    public string ArenaSceneName = "";

    public GameObject MainView;
    public GameObject PlayerView;
    public GameObject FighterView;
    public LeaderboardUI LeaderBoardRef;

    ObjectUIPreviewManager _previewManager;

    void Awake() {
        _previewManager = FindAnyObjectByType<ObjectUIPreviewManager>();
        //    CloseCharacterEditing();

        //     StartCoroutine(WaitForInitalization());
        OpenMainMenu();
    }

    public void StartFight() {
        if (CharacterManager.Instance.SelectedCharacter1 == null && CharacterManager.Instance.SelectedCharacter2 == null) return;

        SceneManager.LoadScene(ArenaSceneName);
        SceneManager.UnloadScene(MainMenuSceneName);
    }

    public void OpenMainMenu() {
        MainView.SetActive(true);
        PlayerView.SetActive(false);
        FighterView.SetActive(false);
        UpdateLeaderboard();
        GetComponentInChildren<BettingUI>().UpdateBettingView();
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
        if (PlayerManager.Instance.PlayerList.Count < 1) return;

        List<Player> sortedList = PlayerManager.Instance.PlayerList.OrderBy(player => player.Points).ToList();
        for (int i = 0; i < sortedList.Count; i++) {
            Player player = sortedList[i];
            LeaderBoardRef.AddItem(player.Name, i+1, player.Points);
        }
    }
}