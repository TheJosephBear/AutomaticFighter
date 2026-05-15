using UnityEngine;

public class AnimationManager : MonoBehaviour {

    public Animator Animator;

    FighterEntity _entity;

    string _currentState = "";

    // prevents trigger spam
    bool _hitTriggered;

    void Awake() {

        _entity = GetComponent<FighterEntity>();
    }

    void Update() {

        HandleEffects();

        // hit animation overrides everything
        if (HasEffect(FighterEffect.Hit))
            return;

        SetAnimationState();
    }

    void HandleEffects() {

        // =========================
        // HIT
        // =========================

        if (HasEffect(FighterEffect.Hit)) {

            if (!_hitTriggered) {

                ShutOffAllStates();

                FiftyFiftyTrigger("Hit", "Hit2");

                _hitTriggered = true;
            }

            return;
        }

        _hitTriggered = false;

        // =========================
        // STUN
        // =========================

        if (HasEffect(FighterEffect.Stunned)) {

            ForceState("Stunned");
            return;
        }

        // =========================
        // POISON
        // =========================

        if (HasEffect(FighterEffect.Poisoned)) {

            // Optional:
            // layer/additive animation later
            // For now just keep regular animation
        }
    }

    void SetAnimationState() {

        string newState =
            GetStateFromDecision(_entity.CurrentDecision);

        // guard against repeated sets
        if (newState == _currentState)
            return;

        ForceState(newState);
    }

    void ForceState(string newState) {

        if (newState == _currentState)
            return;

        // disable previous
        SetBool(_currentState, false);

        // enable new
        SetBool(newState, true);

        _currentState = newState;
    }

    string GetStateFromDecision(FighterDecision decision) {

        switch (decision) {

            case FighterDecision.BasicAttacking:
                return "Attack";

            case FighterDecision.CastSpecialAttack:
                return "Special";

            case FighterDecision.MoveToEnemy:
            case FighterDecision.GetPowerUp:
            case FighterDecision.RunAway:
                return "Walk";

            case FighterDecision.CircleAround:
                return "Strafe";

            case FighterDecision.Waiting:
            case FighterDecision.Defend:
                return "Idle";

            case FighterDecision.Die:
                return "Die";

            default:
                return "Idle";
        }
    }

    bool HasEffect(FighterEffect effect) {

        return _entity.HasEffect(effect);
    }

    void ShutOffAllStates() {

        SetBool("Attack", false);
        SetBool("Special", false);
        SetBool("Walk", false);
        SetBool("Strafe", false);
        SetBool("Idle", false);
        SetBool("Die", false);
        SetBool("Stunned", false);

        _currentState = "";
    }

    void SetBool(string state, bool value) {

        if (string.IsNullOrEmpty(state))
            return;

        // avoids animator re-setting same value
        if (Animator.GetBool(state) == value)
            return;

        Animator.SetBool(state, value);
    }

    public void FireTrigger(string action) {
        switch (action) {
            case "attack":
                FiftyFiftyTrigger("Attack1", "Attack2");
                break;
            case "cast":
                Animator.SetTrigger("Cast1");
                break;
            case "defend":
                Animator.SetTrigger("Defend");
                break;
        }
    }

    void FiftyFiftyTrigger(string trigger1, string trigger2) {
        if (UnityEngine.Random.value <= 0.5f) {
            Animator.SetTrigger(trigger1);
        } else {
            Animator.SetTrigger(trigger2);
        }
    }
}