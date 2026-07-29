using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleGuyAttack : FighterAttackBase {

    public float PurpleGuyLifeTime;
    public float PurpleGuyDamage;
    public float PurpleGuyAttackSpeed;
    public float PurpleGuyMovementSpeed;


    public GameObject PurpleGuyEntityPrefab;
    public GameObject VFXPrefab;
    public GameObject VFXSpringlockPrefab;
    public Vector3 EntitySpawnOffset;
    public Vector3 VFXSpawnOffset;
    public float VFXSpringlockTimer;

    FighterEntity _purpleEntityInstance;

    public override float ExecuteAttack() {
        // Play effect
        Instantiate(VFXPrefab, transform.position + EntitySpawnOffset, Quaternion.identity);
        Instantiate(VFXPrefab, transform.position, Quaternion.identity);
        // Spawn Willie
        _purpleEntityInstance = 
            Instantiate(PurpleGuyEntityPrefab, transform.position + EntitySpawnOffset, Quaternion.identity)
            .GetComponent<FighterEntity>();
        // Initialize Willie
        _purpleEntityInstance.Enemy = transform.root.GetComponent<FighterEntity>().Enemy;
        _purpleEntityInstance.Damage.CurrentValue = PurpleGuyDamage;
        _purpleEntityInstance.AttackSpeed.CurrentValue = PurpleGuyAttackSpeed;
        _purpleEntityInstance.MoveSpeed.CurrentValue = PurpleGuyMovementSpeed;
        _purpleEntityInstance.ApplyEffect(new ActiveEffect {
            Type = FighterEffect.Enrage,
            Duration = 999f
        });

        StartCoroutine(SpringlockCoroutine());

        return 0f;
    }

    IEnumerator SpringlockCoroutine() {
        yield return new WaitForSeconds(PurpleGuyLifeTime);
        if (_purpleEntityInstance == null) yield break;

        StartCoroutine(DestroyAfterWait(VFXSpringlockTimer,
            Instantiate(VFXSpringlockPrefab,
            _purpleEntityInstance.transform.position + EntitySpawnOffset,
            Quaternion.identity)
            ));

        Destroy(_purpleEntityInstance.gameObject);
    }

    IEnumerator DestroyAfterWait(float seconds, GameObject gameObject) {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
