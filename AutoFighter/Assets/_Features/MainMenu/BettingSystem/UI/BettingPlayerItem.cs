using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BettingPlayerItem : MonoBehaviour {

    public GameObject ParticleEffectPrefab;

    public TextMeshProUGUI NameLabel;
    public TextMeshProUGUI TotalPointsLabel;
    public TextMeshProUGUI BetAmountLabel;

    public Image Fighter1BetHighlight;
    public Image Fighter2BetHighlight;
    public TextMeshProUGUI Fighter1BetButtName;
    public TextMeshProUGUI Fighter2BetButtName;

    private Player _player;
    private BettingUI _bettingUI;
    private int _currentBet = 1;
    private int _selectedFighter = 1;

    public void InitializeUI(Player player, BettingUI bettingUI) {
        _player = player;
        _bettingUI = bettingUI;

        NameLabel.text = _player.Name;
        TotalPointsLabel.text = $"Majitel {_player.Points} piv";

        // Safely retrieve existing bet or fallback to default (Fighter 1, 1 Point)
        int choice = 1;
        int amount = 1;

        if (BettingManager.Instance != null) {
            Bet existingBet = BettingManager.Instance.GetPlayerBet(_player);
            choice = existingBet.Choice;
            amount = existingBet.Amount;
        } else {
            Debug.LogWarning("BettingManager.Instance is null! Using default bet values.");
        }

        _selectedFighter = choice;
        _currentBet = Mathf.Clamp(amount, 1, Mathf.Max(1, _player.Points));

        UpdateItemText();
        UpdateSelectionVisuals();
        BettingManager.Instance.RegisterPendingBet(_player, _selectedFighter, 1);
    }

    public void SetFighterNames(string fighter1Name, string fighter2Name) {
        if (Fighter1BetButtName != null) Fighter1BetButtName.text = fighter1Name;
        if (Fighter2BetButtName != null) Fighter2BetButtName.text = fighter2Name;
    }

    void UpdateItemText() {
        BetAmountLabel.text = _currentBet.ToString();
    }

    public void OnRaiseBet() {
        if (_currentBet < _player.Points) {
            _currentBet++;
            UpdateItemText();
            BettingManager.Instance.RegisterPendingBet(_player, _selectedFighter, _currentBet);
        }
    }

    public void OnLowerBet() {
        if (_currentBet > 1) {
            _currentBet--;
            UpdateItemText();
            BettingManager.Instance.RegisterPendingBet(_player, _selectedFighter, _currentBet);
        }
    }

    public void OnBetFighter1() {
        _selectedFighter = 1;
        UpdateSelectionVisuals();
        BettingManager.Instance.RegisterPendingBet(_player, _selectedFighter, _currentBet);
    }

    public void OnBetFighter2() {
        _selectedFighter = 2;
        UpdateSelectionVisuals();
        BettingManager.Instance.RegisterPendingBet(_player, _selectedFighter, _currentBet);
    }

    private void UpdateSelectionVisuals() {
        // Highlights the active fighter choice clearly
        if (_selectedFighter == 1) {
            Fighter1BetHighlight.color = new Color(0.9f, 0.2f, 0.2f, 1f); // Red active
            Fighter2BetHighlight.color = new Color(0.2f, 0.5f, 0.2f, 0.4f); // Green dimmed
        } else {
            Fighter1BetHighlight.color = new Color(0.5f, 0.2f, 0.2f, 0.4f); // Red dimmed
            Fighter2BetHighlight.color = new Color(0.2f, 0.9f, 0.2f, 1f); // Green active
        }
    }

    /// <summary>
    /// GAMBA = ALL IN! Sets bet to max available points
    /// </summary>
    public void OnGambaClick() {
        if (_player.Points <= 0) return;

        SpawnParticleAtCursor();

         _currentBet = _player.Points;
        UpdateItemText();
        BettingManager.Instance.RegisterPendingBet(_player, _selectedFighter, _currentBet);
    }

    void SpawnParticleAtCursor() {
        // Instantiate as a child of the Canvas so it renders in the UI hierarchy
        Canvas TargetCanvas = transform.root.GetComponent<Canvas>();
        GameObject particle = Instantiate(ParticleEffectPrefab, TargetCanvas.transform);

        RectTransform canvasRect = TargetCanvas.GetComponent<RectTransform>();
        Camera uiCamera = TargetCanvas.worldCamera != null ? TargetCanvas.worldCamera : Camera.main;

        // Convert mouse screen position directly to the Canvas plane's world space
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvasRect,
                Input.mousePosition,
                uiCamera,
                out Vector3 worldPoint)) {
            particle.transform.position = worldPoint;
        }
    }
}