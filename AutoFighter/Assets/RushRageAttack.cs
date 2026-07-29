using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushRageAttack : FighterAttackBase {

    public GameObject RushVFX;
    public GameObject TeleportVFX;
    public Vector3 TeleportOffset;
    public Vector3 VFXSpawnOffset;

    public float AttackSpeedMultiplier;

    FighterEntity _entity;
    FighterEntity _enemy;
    GameObject _vfxInstance;

    void Awake() {
        _entity = transform.root.GetComponent<FighterEntity>();
    }

    public override float ExecuteAttack() {
        _enemy = _entity.Enemy;
        // Activate rush (vfx + attack speed)
        StartCoroutine(AttackCoroutine());
        // Teleport to enemy
        StartCoroutine(DestroyGameObjectAfterSeconds(Instantiate(TeleportVFX, transform.position + VFXSpawnOffset, Quaternion.identity), 3f));
        transform.root.GetComponent<Rigidbody>().MovePosition(_enemy.transform.position + TeleportOffset);
        // Enrage
        _entity.ApplyEffect(new ActiveEffect {
            Type = FighterEffect.Enrage,
            Duration = AttackDuration,
        });
        return 0f;
    }

    IEnumerator AttackCoroutine() {
        _vfxInstance = Instantiate(RushVFX, transform);
        _vfxInstance.transform.position += VFXSpawnOffset;
        _entity.AttackSpeed.AddTemporaryMultiplier(AttackSpeedMultiplier);

        yield return new WaitForSeconds(AttackDuration);

        Destroy(_vfxInstance);
        _entity.AttackSpeed.RemoveTemporaryMultiplier(AttackSpeedMultiplier);
    }

    IEnumerator DestroyGameObjectAfterSeconds(GameObject gameObject, float seconds) {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

}
