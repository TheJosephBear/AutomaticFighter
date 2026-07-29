using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour {

    [Header("Fighter 1 UI")]
    public TextMeshProUGUI Name1;
    public Slider HP1;
    public Slider Mana1;

    [Header("Fighter 2 UI")]
    public TextMeshProUGUI Name2;
    public Slider HP2;
    public Slider Mana2;

    // Track max values internally to calculate 0 - 1 normalized slider ratios
    private float _maxHP1 = 1f;
    private float _maxHP2 = 1f;
    private float _maxMana1 = 1f;
    private float _maxMana2 = 1f;



    /// <summary>
    /// Initializes Fighter 1 UI with names and starting stats.
    /// </summary>
    public void InitializeFighter1(string name, float maxHP, float maxMana = 100f) {
        if (Name1 != null) Name1.text = name;
        _maxHP1 = Mathf.Max(1f, maxHP);
        _maxMana1 = Mathf.Max(1f, maxMana);

        UpdateHP1(_maxHP1);
        UpdateMana1(_maxMana1);
    }

    /// <summary>
    /// Initializes Fighter 2 UI with names and starting stats.
    /// </summary>
    public void InitializeFighter2(string name, float maxHP, float maxMana = 100f) {
        if (Name2 != null) Name2.text = name;
        _maxHP2 = Mathf.Max(1f, maxHP);
        _maxMana2 = Mathf.Max(1f, maxMana);

        UpdateHP2(_maxHP2);
        UpdateMana2(_maxMana2);
    }

    #region Direct Normalized Setters (Value passed is between 0 and 1)

    /// <summary> Sets Fighter 1 HP using direct 0-1 ratio. </summary>
    public void SetHP1Normalized(float ratio) {
        if (HP1 != null) HP1.value = Mathf.Clamp01(ratio);
    }

    /// <summary> Sets Fighter 2 HP using direct 0-1 ratio. </summary>
    public void SetHP2Normalized(float ratio) {
        if (HP2 != null) HP2.value = Mathf.Clamp01(ratio);
    }

    /// <summary> Sets Fighter 1 Mana using direct 0-1 ratio. </summary>
    public void SetMana1Normalized(float ratio) {
        if (Mana1 != null) Mana1.value = Mathf.Clamp01(ratio);
    }

    /// <summary> Sets Fighter 2 Mana using direct 0-1 ratio. </summary>
    public void SetMana2Normalized(float ratio) {
        if (Mana2 != null) Mana2.value = Mathf.Clamp01(ratio);
    }

    #endregion

    #region Raw Value Setters (Calculates 0-1 ratio automatically based on max stats)

    /// <summary> Pass raw current HP (e.g., 75 out of 100 HP). </summary>
    public void UpdateHP1(float currentHP) {
        SetHP1Normalized(currentHP / _maxHP1);
    }

    /// <summary> Pass raw current HP (e.g., 50 out of 200 HP). </summary>
    public void UpdateHP2(float currentHP) {
        SetHP2Normalized(currentHP / _maxHP2);
    }

    /// <summary> Pass raw current Mana. </summary>
    public void UpdateMana1(float currentMana) {
        SetMana1Normalized(currentMana / _maxMana1);
    }

    /// <summary> Pass raw current Mana. </summary>
    public void UpdateMana2(float currentMana) {
        SetMana2Normalized(currentMana / _maxMana2);
    }

    #endregion
}