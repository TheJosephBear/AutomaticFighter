using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BettingPlayerItem : MonoBehaviour {

    public TextMeshProUGUI NameLabel;
    public TextMeshProUGUI TotalPointsLabel;
    public TextMeshProUGUI BetAmountLabel;

    // Colored panels or buttons for selecting the fighter to bet on
    public Image Fighter1BetHighlight;
    public Image Fighter2BetHighlight;

    private Player _player;
    private BettingUI _bettingUI; // The main script handling the betting layout
    private int _currentBet = 10; // Default or starting bet

    public void InitializeUI(Player player, BettingUI bettingUI) {
        _player = player;
        _bettingUI = bettingUI;

        NameLabel.text = _player.Name;
        TotalPointsLabel.text = $"Majitel {_player.Points} piv"; // Translates to "Owner of X beers"

        _currentBet = Mathf.Min(10, _player.Points); // Ensure they have enough points
        UpdateItemText();
    }

    void UpdateItemText() {
        BetAmountLabel.text = _currentBet.ToString();
    }

    public void OnRaiseBet() {
        if (_currentBet < _player.Points) {
            _currentBet++;
            UpdateItemText();
        }
    }

    public void OnLowerBet() {
        if (_currentBet > 1) {
            _currentBet--;
            UpdateItemText();
        }
    }

    public void OnBetFighter1() {
        // Toggle visual feedback (e.g., green if active, red/dark if not)
        Fighter1BetHighlight.color = Color.green;
        Fighter2BetHighlight.color = Color.red;
        _bettingUI.BetOnFighter1(_player, _currentBet);
    }

    public void OnBetFighter2() {
        Fighter2BetHighlight.color = Color.green;
        Fighter1BetHighlight.color = Color.red;
        _bettingUI.BetOnFighter2(_player, _currentBet);
    }

    public void OnGambaClick() {
        _bettingUI.OnGAMBA(_player, _currentBet);
    }
}