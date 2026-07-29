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
    FighterEntity _figherEntityScript1 = null;
    FighterEntity _figherEntityScript2 = null;

    private Coroutine _cameraSwitchCoroutine;
    private bool _isFightActive = false;

    void Awake() {
        Initialize();
    }

    public void Initialize() {
        _arenaManager = FindAnyObjectByType<ArenaManager>();
        _bettingManager = FindAnyObjectByType<BettingManager>();
        _hudManager = FindAnyObjectByType<HUDManager>();
        CutsceneManager cutsceneman = FindAnyObjectByType<CutsceneManager>();

        SpawnFighters();
        GameOverUI.SetActive(false);
        _hudManager.ToggleUI(false);
        SpectatorVcamReference.LookAt = _figherEntityScript1.gameObject.transform;

        cutsceneman.IntroductionScene(_figherEntityScript1.transform, _figherEntityScript2.transform, () => {
            _hudManager.ToggleUI(true);
            InitializeHUD();
            StartCoroutine(DelayStartCoroutine());
        });
    }

    IEnumerator DelayStartCoroutine() {
        yield return new WaitForSeconds(2f);
        StartFight();
    }

    void StartFight() {
        _figherEntityScript1.Enemy = _figherEntityScript2;
        _figherEntityScript2.Enemy = _figherEntityScript1;
        StartCameraSwitching(_figherEntityScript1.transform, _figherEntityScript2.transform);
    }

    private void SpawnFighters() {
        Character c1 = CharacterManager.Instance.SelectedCharacter1;
        Character c2 = CharacterManager.Instance.SelectedCharacter2;

        // Get the matching prefabs from CharacterManager's CharacterPrefabList
        if (c1 != null) {
            GameObject prefab1 = GetPrefabForModel(c1.CharacterModel);
            if (prefab1 != null) {
                Vector3 spawnPos = SpawnPoint1 != null ? SpawnPoint1.position : Vector3.left * 2f;
                Quaternion spawnRot = SpawnPoint1 != null ? SpawnPoint1.rotation : Quaternion.identity;
                Fighter1 = Instantiate(prefab1, spawnPos, spawnRot);

                _figherEntityScript1 = Fighter1.GetComponent<FighterEntity>();
                _figherEntityScript1.HP.BaseValue = c1.HP;
                _figherEntityScript1.HP.CurrentValue = c1.HP;
                _figherEntityScript1.Damage.BaseValue = c1.DMG;
                _figherEntityScript1.Damage.CurrentValue = c1.DMG;
                _figherEntityScript1.AttackSpeed.BaseValue = c1.AS;
                _figherEntityScript1.AttackSpeed.CurrentValue = c1.AS;
                _figherEntityScript1.MoveSpeed.BaseValue = c1.MS;
                _figherEntityScript1.MoveSpeed.CurrentValue = c1.MS;
            }
        }

        if (c2 != null) {
            GameObject prefab2 = GetPrefabForModel(c2.CharacterModel);
            if (prefab2 != null) {
                Vector3 spawnPos = SpawnPoint2 != null ? SpawnPoint2.position : Vector3.right * 2f;
                Quaternion spawnRot = SpawnPoint2 != null ? SpawnPoint2.rotation : Quaternion.identity;
                Fighter2 = Instantiate(prefab2, spawnPos, spawnRot);

                _figherEntityScript2 = Fighter2.GetComponent<FighterEntity>();
                _figherEntityScript2.HP.BaseValue = c2.HP;
                _figherEntityScript2.HP.CurrentValue = c2.HP;
                _figherEntityScript2.Damage.BaseValue = c2.DMG;
                _figherEntityScript2.Damage.CurrentValue = c2.DMG;
                _figherEntityScript2.AttackSpeed.BaseValue = c2.AS;
                _figherEntityScript2.AttackSpeed.CurrentValue = c2.AS;
                _figherEntityScript2.MoveSpeed.BaseValue = c2.MS;
                _figherEntityScript2.MoveSpeed.CurrentValue = c2.MS;
            }
        }

        if (_figherEntityScript1 != null && _figherEntityScript2 != null) {
        //    _figherEntityScript1.Enemy = _figherEntityScript2;
         //   _figherEntityScript2.Enemy = _figherEntityScript1;

            _figherEntityScript1.OnHPChange += _hudManager.UpdateHP1;
            _figherEntityScript1.OnHit += () => {  
                    _figherEntityScript2.Mana.CurrentValue += 10;          
            };
            _figherEntityScript2.OnHit += () => {
                    _figherEntityScript1.Mana.CurrentValue += 10;       
            };
            _figherEntityScript2.OnHPChange += _hudManager.UpdateHP2;
            _figherEntityScript1.OnManaChange += _hudManager.UpdateMana1;
            _figherEntityScript2.OnManaChange += _hudManager.UpdateMana2;
            _figherEntityScript1.OnDeath += () => OnFightOver(c1, _figherEntityScript2);
            _figherEntityScript2.OnDeath += () => OnFightOver(c2, _figherEntityScript1);
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
            yield return new WaitForSeconds(Random.Range(3f, 5f));

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

    public void OnFightOver(Character defeatedCharacter, FighterEntity winnerCharacter) {
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

        CutsceneManager cutsceneman = FindAnyObjectByType<CutsceneManager>();
        winnerCharacter.Enemy = null;
        winnerCharacter.GetComponent<FighterAI>().enabled = false;
        winnerCharacter.enabled = false;
        cutsceneman.WinScene(winnerCharacter.gameObject , () => {
            Time.timeScale = 0f;
            if (GameOverUI != null) {
                GameOverUI.SetActive(true);
            }
        });
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
