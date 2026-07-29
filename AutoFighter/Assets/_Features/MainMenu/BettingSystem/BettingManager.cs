using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Bet {
    public int Choice; // 1 for Fighter 1, 2 for Fighter 2
    public int Amount;

    public Bet(int choice, int amount) {
        Choice = choice;
        Amount = amount;
    }
}

public class BettingManager : Singleton<BettingManager> {

    // Map Player -> Active Bet
    private Dictionary<Player, Bet> _currentRoundBets = new Dictionary<Player, Bet>();

    /// <summary>
    /// Set or update the selection (Fighter 1 or 2) and amount for a player.
    /// </summary>
    public void RegisterPendingBet(Player player, int choice, int amount) {
        if (player == null) return;
        int validAmount = Mathf.Clamp(amount, 0, player.Points);
        _currentRoundBets[player] = new Bet(choice, validAmount);
        print($"{player} is betting {amount} on {choice} ");
    }

    /// <summary>
    /// Locks in a player's bet (including ALL-IN from GAMBA) and deducts points.
    /// </summary>
    public bool FinalizeBet(Player player, int choice, int amount) {
        if (player == null || player.Points <= 0) {
            Debug.LogWarning($"{player?.Name} has no points to bet!");
            return false;
        }

        int finalAmount = Mathf.Min(amount, player.Points);
        _currentRoundBets[player] = new Bet(choice, finalAmount);

        // Deduct points instantly for the active round
        PlayerManager.Instance.AddPlayerPoint(player, -finalAmount);
        Debug.Log($"{player.Name} locked in a bet of {finalAmount} points on Fighter {choice}!");
        return true;
    }

    /// <summary>
    /// Resolves payout at the end of a match.
    /// </summary>
    public void ResolveRoundBets(int winningFighterIndex, float payoutMultiplier = 2.0f) {
        foreach (KeyValuePair<Player, Bet> entry in _currentRoundBets) {
            Player player = entry.Key;
            Bet bet = entry.Value;

            if (bet.Choice == winningFighterIndex) {
                int winnings = Mathf.RoundToInt(bet.Amount * payoutMultiplier);
                PlayerManager.Instance.AddPlayerPoint(player, winnings);
                Debug.Log($"{player.Name} WON! Received {winnings} points.");
            } else {
                Debug.Log($"{player.Name} LOST their bet of {bet.Amount} points.");
            }
        }

        ResetRoundBets();
    }

    public void ResetRoundBets() {
        _currentRoundBets.Clear();
    }

    public Bet GetPlayerBet(Player player) {
        if (_currentRoundBets.TryGetValue(player, out Bet bet)) {
            return bet;
        }
        return new Bet(1, 1);
    }

}