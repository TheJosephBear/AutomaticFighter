using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallAttack : FighterAttackBase {

    public GameObject ProjectilePrefab;
    public Vector3 SpawnOffset;
    public float ProjectileSpeed;


    public override float ExecuteAttack() {
        CurrentDamage = BaseDamage + _entity.Damage.CurrentValue;
        FireBallProjectile projectile = Instantiate(
            ProjectilePrefab,
            transform.position + transform.TransformDirection(SpawnOffset),
            transform.rotation
        ).GetComponent<FireBallProjectile>();

        projectile.Initialize(CurrentDamage, KnockbackStrength, ProjectileSpeed);

        return GetAttackDuration();
    }

}
