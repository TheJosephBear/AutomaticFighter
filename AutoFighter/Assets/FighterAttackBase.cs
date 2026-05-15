using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FighterAttackBase : MonoBehaviour {

    public float BaseDamage = 10f;
    public float KnockbackStrength = 5f;
    public float AttackDuration = 0.1f;

    protected float CurrentDamage;
    protected FighterEntity _entity;

    protected virtual void Awake() {
        _entity = transform.root.GetComponent<FighterEntity>();
    }

    // CurrDamange = baseDamage + damage multiplier of the entity
    public abstract float ExecuteAttack();

    public float GetAttackDuration() {
        return AttackDuration;
    }
}
