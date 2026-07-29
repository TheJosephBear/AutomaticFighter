using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAI : MonoBehaviour {

    public bool AIBrain = true;

    [Header("Chances")]
    [Range(0, 1)]
    public float CircleChance = 0.5f;

    [Range(0, 1)]
    public float WaitChance = 0.15f;

    [Range(0, 1)]
    public float DeterminationChance = 0.5f;

    [Range(0, 1)]
    public float SpecialCastChance = 0.7f;

    [Range(0, 1)]
    public float DefendChance = 0.3f;

    [Header("Reconsideration")]
    [Tooltip("How often AI reconsiders while moving.")]
    public float ReconsiderInterval = 0.5f;

    [Range(0, 1)]
    [Tooltip("Chance to rethink while moving.")]
    public float ReconsiderChance = 0.4f;

    [Header("Distances")]
    public float FightingDistance = 1.5f;
    public float CircleDistance = 3f;
    public float RunAwayDistance = 5f;
    public float MinimumDistance = 2f;

    [Header("Durations")]
    public float CircleDuration = 2f;
    public float WaitMin = 0.3f;
    public float WaitMax = 1f;
    public float RunAwayMin = 1f;
    public float RunAwayMax = 3f;

    [Header("Behavior")]
    public float FearHPPercent = 0.25f;

    FighterEntity _entity;

    bool _locked;
    float _timer;

    FighterDecision _decision;

    float _reconsiderTimer;

    void Awake() {

        _entity = GetComponent<FighterEntity>();
    }

    void Update() {

        if (!AIBrain)
            return;

        if (_entity.IsDead)
            return;

        HandleDecision();
    }

    void KeepDistance() {
        Vector3 selfPos = transform.position;
        Vector3 enemyPos = _entity.Enemy.transform.position;

        // ignore height (Y axis)
        selfPos.y = 0f;
        enemyPos.y = 0f;

        float horizontalDistance = Vector3.Distance(selfPos, enemyPos);

        if (horizontalDistance < MinimumDistance) {

            Unlock();

            LockDecision(
                FighterDecision.RunAway,
                Random.Range(RunAwayMin, RunAwayMax)
            );
        }
    }

    void HandleDecision() {

        KeepDistance();

        if (_locked) {

            TickLockedDecision();
            return;
        }

        MakeDecision();
    }

    void MakeDecision() {

        if (_entity.Enemy == null)
            return;

        float distance =
            _entity.DistanceToEnemy();

        bool lowHP =
            _entity.HP.CurrentValue <
            _entity.HP.GetMaxValue() * FearHPPercent;

        bool fullMana =
            _entity.Mana.CurrentValue >=
            _entity.Mana.GetMaxValue();

        bool enraged = _entity.HasEffect(FighterEffect.Enrage);

        // Enrage override
        if (enraged) {
            // no waiting / circling / hesitation
            // only fight or chase

            if (distance <= FightingDistance) {

                float duration = _entity.BasicAttack();

                LockDecision(
                    FighterDecision.BasicAttacking,
                    duration
                );

                return;
            }

            LockDecision(
                FighterDecision.MoveToEnemy,
                0.2f
            );

            return;
        }

        // Falling override
        if(GetComponent<Rigidbody>().velocity.y < -0.1f) {
            LockDecision(
                FighterDecision.Waiting,
                Random.Range(WaitMin, WaitMax)
            );

            return;
        }

        // =========================
        // WAIT
        // =========================

        if (Random.value < WaitChance) {

            LockDecision(
                FighterDecision.Waiting,
                Random.Range(WaitMin, WaitMax)
            );

            return;
        }

        // =========================
        // RUN
        // =========================

        if (lowHP &&
            distance < RunAwayDistance &&
            Random.value > DeterminationChance) {

            LockDecision(
                FighterDecision.RunAway,
                Random.Range(RunAwayMin, RunAwayMax)
            );

            return;
        }

        // =========================
        // SPECIAL
        // =========================

        if (fullMana &&
            Random.value < SpecialCastChance) {

            float duration =
                _entity.SpecialAttack();

            LockDecision(
                FighterDecision.CastSpecialAttack,
                duration
            );

            return;
        }

        // =========================
        // BASIC ATTACK
        // =========================

        if (distance <= FightingDistance) {
            if(Random.value < DefendChance) {
                float duration =
                _entity.Defend();

                LockDecision(
                    FighterDecision.Defend,
                    duration
                );
            } else {
                float duration =
                _entity.BasicAttack();

                LockDecision(
                    FighterDecision.BasicAttacking,
                    duration
                );
            }

            return;
        }

        // =========================
        // MOVE / CIRCLE
        // =========================

        if (Random.value < CircleChance) {

            LockDecision(
                FighterDecision.CircleAround,
                CircleDuration
            );
        } else {

            LockDecision(
                FighterDecision.MoveToEnemy,
                999f
            );
        }
    }

    void TickLockedDecision() {

        _timer -= Time.deltaTime;

        switch (_decision) {

            case FighterDecision.MoveToEnemy:

                TickMoveToEnemy();
                break;

            case FighterDecision.CircleAround:

                _entity.CircleEnemy(CircleDistance);
                break;

            case FighterDecision.RunAway:

                _entity.RunAway();
                break;

            case FighterDecision.Waiting:

                _entity.Wait();
                break;
        }

        if (_timer <= 0f) {
            Unlock();
        }
    }

    void TickMoveToEnemy() {

        // reached attack range
        if (_entity.DistanceToEnemy() <= FightingDistance) {

            Unlock();
            return;
        }

        _entity.MoveToEnemy();

        // =========================
        // RECONSIDER LOGIC
        // =========================

        _reconsiderTimer -= Time.deltaTime;

        if (_reconsiderTimer > 0f)
            return;

        _reconsiderTimer = ReconsiderInterval;

        if (Random.value > ReconsiderChance)
            return;

        float distance =
            _entity.DistanceToEnemy();

        bool lowHP =
            _entity.HP.CurrentValue <
            _entity.HP.GetMaxValue() * FearHPPercent;

        bool fullMana =
            _entity.Mana.CurrentValue >=
            _entity.Mana.GetMaxValue();

        // RUN AWAY

        if (lowHP &&
            distance < RunAwayDistance &&
            Random.value > DeterminationChance) {

            LockDecision(
                FighterDecision.RunAway,
                Random.Range(RunAwayMin, RunAwayMax)
            );

            return;
        }

        // SPECIAL

        if (fullMana &&
            Random.value < SpecialCastChance) {

            float duration =
                _entity.SpecialAttack();

            LockDecision(
                FighterDecision.CastSpecialAttack,
                duration
            );

            return;
        }

        // CIRCLE

        if (Random.value < CircleChance) {

            LockDecision(
                FighterDecision.CircleAround,
                CircleDuration
            );

            return;
        }
    }

    void LockDecision(FighterDecision decision, float duration) {

        _decision = decision;

        _entity.SetDecision(decision);

        _timer = duration;

        _locked = true;

        _reconsiderTimer = ReconsiderInterval;
    }

    void Unlock() {

        _locked = false;
    }

    public void StopCurrentDecision() {

        Unlock();
    }
}