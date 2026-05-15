using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FighterMovement))]
[RequireComponent(typeof(FighterAI))]
[RequireComponent(typeof(FighterAttackManager))]
public class FighterEntity : MonoBehaviour {

    #region References

    public FighterEntity Enemy;

    Rigidbody _rb;

    FighterMovement _movement;
    FighterAI _ai;
    FighterAttackManager _attackManager;
    FighterVFX _vfx;

    #endregion

    #region Stats

    [Header("Stats")]
    public StatValue HP;
    public StatValue Mana;
    public StatValue HPRegen;
    public StatValue ManaRegen;
    public StatValue Damage;
    public StatValue AttackSpeed;
    public StatValue MoveSpeed;

    #endregion

    #region State

    public FighterDecision CurrentDecision { get; private set; }

    public bool IsDead { get; private set; }
    public bool IsBusy { get; private set; }

    public Action<FighterEntity> OnDeath;

    #endregion

    #region Effects

    public List<ActiveEffect> Effects = new();

    #endregion

    void Awake() {

        _rb = GetComponent<Rigidbody>();

        _movement = GetComponent<FighterMovement>();
        _ai = GetComponent<FighterAI>();
        _attackManager = GetComponent<FighterAttackManager>();
        _vfx = GetComponent<FighterVFX>();

        SyncStats();

        CurrentDecision = FighterDecision.Waiting;
    }

    void Update() {

        if (IsDead)
            return;

        HandleRegeneration();
        HandleEffects();
        // Keep looking at him all the time just to be sure
        _movement.LookAt(Enemy.transform.position);
    }

    void SyncStats() {

        HP.ForceSync();
        Mana.ForceSync();

        HPRegen.ForceSync();
        ManaRegen.ForceSync();

        Damage.ForceSync();
        AttackSpeed.ForceSync();
        MoveSpeed.ForceSync();
    }

    #region ===== AI API =====

    public void SetDecision(FighterDecision decision) {

        CurrentDecision = decision;
    }

    public void MoveToEnemy() {

        if (Enemy == null)
            return;

        _movement.MoveToward(
            Enemy.transform.position,
            MoveSpeed.CurrentValue
        );
    }

    public void CircleEnemy(float preferredDistance) {

        if (Enemy == null)
            return;

        _movement.CircleAround(
            Enemy.transform.position,
            MoveSpeed.CurrentValue,
            preferredDistance
        );
    }

    public void RunAway() {

        if (Enemy == null)
            return;

        Vector3 direction =
            (transform.position - Enemy.transform.position).normalized;

        _movement.MoveInDirection(
            direction,
            MoveSpeed.CurrentValue
        );

        _movement.LookAt(Enemy.transform.position);
    }

    public float BasicAttack() {

        if (IsBusy)
            return 0f;

        IsBusy = true;

        CurrentDecision = FighterDecision.BasicAttacking;

        _movement.StopMovement();

        GetComponent<AnimationManager>().FireTrigger("attack"); // ew
        StartCoroutine(AttackAfterWait(0.2f));

        float duration = AttackSpeed.CurrentValue;

        StartCoroutine(BusyRoutine(duration));

        return duration;
    }

    // EWWWWWWWWWWW
    IEnumerator AttackAfterWait(float wait, bool cast = false) {
        yield return new WaitForSeconds(wait);
        if (cast) {
            _attackManager.CastRandomAttack();

        } else {
            _attackManager.CastBasicAttack();
        }
    }

    public float SpecialAttack() {

        if (IsBusy)
            return 0f;

        if (Mana.CurrentValue < Mana.GetMaxValue())
            return 0f;

        IsBusy = true;

        CurrentDecision = FighterDecision.CastSpecialAttack;

        GetComponent<AnimationManager>().FireTrigger("cast"); // ew

        float duration = 0.8f;

        StartCoroutine(AttackAfterWait(0.2f, cast: true));

        _movement.StopMovement();
        Mana.CurrentValue = 0f;

        StartCoroutine(BusyRoutine(duration));

        return duration;
    }

    public void Wait() {

        _movement.StopMovement();
    }

    #endregion

    #region Combat

    public void ApplyDamage(float damage, float knockbackStrength) {

        if (IsDead)
            return;

        HP.Decrease(damage);

        _movement.ApplyKnockback(knockbackStrength);

        _vfx.PlayRandomHitEffect();

        ApplyEffect(new ActiveEffect {
            Type = FighterEffect.Hit,
            Duration = 0.2f,
        });

        if (HP.CurrentValue <= 0f) {
            Die();
        }
    }

    void Die() {

        if (IsDead)
            return;

        IsDead = true;

        CurrentDecision = FighterDecision.Die;

        _movement.StopMovement();

        _rb.isKinematic = true;

        OnDeath?.Invoke(this);
    }

    #endregion

    #region Effects

    public void ApplyEffect(ActiveEffect effect) {

        Effects.Add(effect);
    }

    void HandleEffects() {

        for (int i = Effects.Count - 1; i >= 0; i--) {

            ActiveEffect effect = Effects[i];

            effect.Duration -= Time.deltaTime;

            switch (effect.Type) {
                case FighterEffect.Poisoned:
                    break;

                case FighterEffect.Hit:
                    break;

                case FighterEffect.Stunned:
                    break;
            }

            if (effect.Duration <= 0f) {
                Effects.RemoveAt(i);
            }
        }
    }

    public bool HasEffect(FighterEffect effect) {

        for (int i = 0; i < Effects.Count; i++) {

            if (Effects[i].Type == effect)
                return true;
        }

        return false;
    }

    #endregion

    #region Utility

    public float DistanceToEnemy() {

        if (Enemy == null)
            return Mathf.Infinity;

        return Vector3.Distance(
            transform.position,
            Enemy.transform.position
        );
    }

    void HandleRegeneration() {

        float delta = Time.deltaTime;

        if (HP.CurrentValue > 0f) {
            HP.Increase(HPRegen.CurrentValue * delta);
        }

        if (Mana.CurrentValue < Mana.GetMaxValue()) {
            Mana.Increase(ManaRegen.CurrentValue * delta);
        }
    }

    IEnumerator BusyRoutine(float duration) {

        yield return new WaitForSeconds(duration);

        IsBusy = false;
    }

    #endregion
}

public enum FighterEffect {
    Hit,
    Stunned,
    Poisoned,
    Invincible, // No damage
    Immune, // No negative effects
    Enrage, // Bigger attack speed
    Empowered, // Bigger damage
    Scared, // Run away in random direction + spam random actions - panic
}

public class ActiveEffect {
    public FighterEffect Type;
    public float Duration;
    public float Timer;
}

public enum FighterDecision {
    Waiting,
    MoveToEnemy,
    BasicAttacking,
    Defend,
    RunAway,
    CircleAround,
    GetPowerUp,
    CastSpecialAttack,
    Die,
}

[Serializable]
public class StatValue : ISerializationCallbackReceiver {

    [SerializeField]
    float _baseValue;

    [SerializeField]
    float _currentValue;

    [SerializeField]
    float _permanentMultiplier = 1f;

    [SerializeField]
    float _temporaryMultiplier = 1f;

    public float BaseValue {
        get => _baseValue;
        set {
            _baseValue = value;

            // Optional:
            // keep current synced when base changes in runtime
            if (!Application.isPlaying)
                ResetCurrent();
        }
    }

    public float CurrentValue {
        get => _currentValue;
        set => _currentValue = Mathf.Clamp(value, 0f, GetMaxValue());
    }

    /// <summary>
    /// Final calculated max value.
    /// </summary>
    public float GetMaxValue() {
        return _baseValue * _permanentMultiplier * _temporaryMultiplier;
    }

    /// <summary>
    /// Adds a temporary multiplier.
    /// Example:
    /// Rage buff, potion, temporary aura.
    /// </summary>
    public void AddTemporaryMultiplier(float multiplier) {

        float oldMax = GetMaxValue();

        _temporaryMultiplier *= multiplier;

        float newMax = GetMaxValue();

        float ratio = oldMax > 0f ? _currentValue / oldMax : 1f;

        _currentValue = Mathf.Clamp(ratio * newMax, 0f, newMax);
    }

    /// <summary>
    /// Removes temporary multiplier.
    /// </summary>
    public void RemoveTemporaryMultiplier(float multiplier) {

        float oldMax = GetMaxValue();

        _temporaryMultiplier /= multiplier;

        float newMax = GetMaxValue();

        float ratio = oldMax > 0f ? _currentValue / oldMax : 1f;

        _currentValue = Mathf.Clamp(ratio * newMax, 0f, newMax);
    }

    /// <summary>
    /// Adds a permanent multiplier.
    /// Example:
    /// Level up, passive skill, equipment.
    /// </summary>
    public void AddPermanentMultiplier(float multiplier) {

        _permanentMultiplier *= multiplier;

        ClampCurrent();
    }

    /// <summary>
    /// Removes permanent multiplier.
    /// </summary>
    public void RemovePermanentMultiplier(float multiplier) {

        if (multiplier == 0f)
            return;

        _permanentMultiplier /= multiplier;

        ClampCurrent();
    }

    /// <summary>
    /// Damage / spending resource.
    /// </summary>
    public void Decrease(float amount) {

        CurrentValue -= amount;
    }

    /// <summary>
    /// Healing / restoring resource.
    /// </summary>
    public void Increase(float amount) {

        CurrentValue += amount;
    }

    /// <summary>
    /// Restore to max.
    /// </summary>
    public void ResetCurrent() {

        _currentValue = GetMaxValue();
    }

    void ClampCurrent() {

        _currentValue = Mathf.Clamp(_currentValue, 0f, GetMaxValue());
    }

    // =========================================
    // UNITY SERIALIZATION CALLBACKS
    // =========================================

    public void OnBeforeSerialize() {

    }

    public void OnAfterDeserialize() {

        // Called automatically after inspector deserialization

        if (_currentValue <= 0f)
            ResetCurrent();
    }

    public void ForceSync() {

        _permanentMultiplier = Mathf.Max(0f, _permanentMultiplier);
        _temporaryMultiplier = Mathf.Max(0f, _temporaryMultiplier);

        _currentValue = GetMaxValue();
    }
}