using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaManager : MonoBehaviour {

    public CinemachineVirtualCamera SpectatorVcamReference;  
    public GameObject GameOverUI;

    [Header("Fighter Spawn Points")]
    public Transform SpawnPoint1;
    public Transform SpawnPoint2;

    [HideInInspector]
    public GameObject Fighter1;
    [HideInInspector]
    public GameObject Fighter2;

    ArenaManager _arenaManager;
    BettingManager _bettingManager;
    HUDManager _hudManager;

    private Coroutine _cameraSwitchCoroutine;
    private bool _isFightActive = false;

    void Awake() {
        Initialize();
    }

    public void Initialize() {
        _arenaManager = FindAnyObjectByType<ArenaManager>();
        _bettingManager = FindAnyObjectByType<BettingManager>();
        _hudManager = FindAnyObjectByType<HUDManager>();

        SpawnFighters();
        InitializeHUD();
        GameOverUI.SetActive(false);
    }

    private void SpawnFighters() {
        Character c1 = CharacterManager.Instance.SelectedCharacter1;
        Character c2 = CharacterManager.Instance.SelectedCharacter2;
        FighterEntity figherEntityScript1 = null;
        FighterEntity figherEntityScript2 = null;

        // Get the matching prefabs from CharacterManager's CharacterPrefabList
        if (c1 != null) {
            GameObject prefab1 = GetPrefabForModel(c1.CharacterModel);
            if (prefab1 != null) {
                Vector3 spawnPos = SpawnPoint1 != null ? SpawnPoint1.position : Vector3.left * 2f;
                Quaternion spawnRot = SpawnPoint1 != null ? SpawnPoint1.rotation : Quaternion.identity;
                Fighter1 = Instantiate(prefab1, spawnPos, spawnRot);

                figherEntityScript1 = Fighter1.GetComponent<FighterEntity>();
                figherEntityScript1.HP.BaseValue = c1.HP;
                figherEntityScript1.HP.CurrentValue = c1.HP;
                figherEntityScript1.Damage.BaseValue = c1.DMG;
                figherEntityScript1.Damage.CurrentValue = c1.DMG;
                figherEntityScript1.AttackSpeed.BaseValue = c1.AS;
                figherEntityScript1.AttackSpeed.CurrentValue = c1.AS;
                figherEntityScript1.MoveSpeed.BaseValue = c1.MS;
                figherEntityScript1.MoveSpeed.CurrentValue = c1.MS;
            }
        }

        if (c2 != null) {
            GameObject prefab2 = GetPrefabForModel(c2.CharacterModel);
            if (prefab2 != null) {
                Vector3 spawnPos = SpawnPoint2 != null ? SpawnPoint2.position : Vector3.right * 2f;
                Quaternion spawnRot = SpawnPoint2 != null ? SpawnPoint2.rotation : Quaternion.identity;
                Fighter2 = Instantiate(prefab2, spawnPos, spawnRot);

                figherEntityScript2 = Fighter2.GetComponent<FighterEntity>();
                figherEntityScript2.HP.BaseValue = c2.HP;
                figherEntityScript2.HP.CurrentValue = c2.HP;
                figherEntityScript2.Damage.BaseValue = c2.DMG;
                figherEntityScript2.Damage.CurrentValue = c2.DMG;
                figherEntityScript2.AttackSpeed.BaseValue = c2.AS;
                figherEntityScript2.AttackSpeed.CurrentValue = c2.AS;
                figherEntityScript2.MoveSpeed.BaseValue = c2.MS;
                figherEntityScript2.MoveSpeed.CurrentValue = c2.MS;
            }
        }

        if (figherEntityScript1 != null && figherEntityScript2 != null) {
            figherEntityScript1.Enemy = figherEntityScript2;
            figherEntityScript2.Enemy = figherEntityScript1;
            StartCameraSwitching(figherEntityScript1.transform, figherEntityScript2.transform);

            figherEntityScript1.OnHPChange += _hudManager.UpdateHP1;
            figherEntityScript1.OnHit += () => {  
                    figherEntityScript2.Mana.CurrentValue += 10;          
            };
            figherEntityScript2.OnHit += () => {
                    figherEntityScript1.Mana.CurrentValue += 10;       
            };
            figherEntityScript2.OnHPChange += _hudManager.UpdateHP2;
            figherEntityScript1.OnManaChange += _hudManager.UpdateMana1;
            figherEntityScript2.OnManaChange += _hudManager.UpdateMana2;
            figherEntityScript1.OnDeath += () => OnFightOver(c1);
            figherEntityScript2.OnDeath += () => OnFightOver(c2);
        }

    }

    public void StartCameraSwitching(Transform fighter1, Transform fighter2) {
        _isFightActive = true;

        // Stop any existing coroutine to prevent running multiple loops
        if (_cameraSwitchCoroutine != null) {
            StopCoroutine(_cameraSwitchCoroutine);
        }

        _cameraSwitchCoroutine = StartCoroutine(CameraSwitchingCoroutine(fighter1, fighter2));
    }

    IEnumerator CameraSwitchingCoroutine(Transform lookAtOne, Transform lookAtTwo) {
        Transform currentTarget = lookAtOne;

        while (_isFightActive) {
            // Assign active target
            SpectatorVcamReference.LookAt = currentTarget;
            SpectatorVcamReference.Follow = currentTarget;

            // Wait for a random duration
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            // Alternate target
            currentTarget = (currentTarget == lookAtOne) ? lookAtTwo : lookAtOne;
        }
    }

    private void InitializeHUD() {
        if (_hudManager == null || CharacterManager.Instance == null) return;

        Character c1 = CharacterManager.Instance.SelectedCharacter1;
        Character c2 = CharacterManager.Instance.SelectedCharacter2;

        if (c1 != null) {
            _hudManager.InitializeFighter1(c1.Name, c1.HP);
        }

        if (c2 != null) {
            _hudManager.InitializeFighter2(c2.Name, c2.HP);
        }
    }

    private GameObject GetPrefabForModel(CharacterModel model) {
        foreach (var pairing in CharacterManager.Instance.CharacterPrefabList) {
            if (pairing.CharacterModelEnum == model) {
                return pairing.CharacterEntityPrefab;
            }
        }
        Debug.LogWarning($"No prefab mapping found for model: {model}");
        return null;
    }

    public void OnFightOver(Character defeatedCharacter) {
        if (!_isFightActive) return; // Prevents double-firing if both die on the same frame
        _isFightActive = false;

        if (_cameraSwitchCoroutine != null) {
            StopCoroutine(_cameraSwitchCoroutine);
            _cameraSwitchCoroutine = null;
        }

        // Determine winning fighter index (1 for Fighter 1, 2 for Fighter 2)
        int winningFighterIndex = 0;
        Character c1 = CharacterManager.Instance != null ? CharacterManager.Instance.SelectedCharacter1 : null;
        Character c2 = CharacterManager.Instance != null ? CharacterManager.Instance.SelectedCharacter2 : null;

        if (defeatedCharacter == c1) {
            winningFighterIndex = 2; // Fighter 2 Won
        } else if (defeatedCharacter == c2) {
            winningFighterIndex = 1; // Fighter 1 Won
        }

        // Resolve bets, increment win counters, and distribute points
        ResolveMatchResults(winningFighterIndex);

        Debug.Log($"Fight over! Winner is Fighter {winningFighterIndex}. Spectator camera stopped.");

        Time.timeScale = 0f;
        if (GameOverUI != null) {
            GameOverUI.SetActive(true);
        }
    }

    private void ResolveMatchResults(int winningFighterIndex) {
        var bettingMgr = _bettingManager != null ? _bettingManager : BettingManager.Instance;

        if (bettingMgr == null) {
            Debug.LogWarning("BettingManager instance not found to resolve bets!");
            return;
        }

        // 1. Increment WinCount for players who bet on the winning fighter
        if (PlayerManager.Instance != null && PlayerManager.Instance.PlayerList != null) {
            foreach (Player player in PlayerManager.Instance.PlayerList) {
                Bet playerBet = bettingMgr.GetPlayerBet(player);
                if (playerBet.Choice == winningFighterIndex) {
                    PlayerManager.Instance.AddPlayerWin(player);
                } else {
                    PlayerManager.Instance.AddPlayerPoint(player, -playerBet.Amount);
                }
            }
        }

        // 2. Resolve payout multiplier and award points through BettingManager
        bettingMgr.ResolveRoundBets(winningFighterIndex, 2.0f);
    }

    public void OnExit() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
        SceneManager.UnloadScene("ArenaScene");
    }


}
