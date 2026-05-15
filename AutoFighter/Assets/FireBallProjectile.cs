using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallProjectile : MonoBehaviour {

    public GameObject ImpactVFX;

    float _dmg;
    float _knockback;
    float _speed;

    void Update() {
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    public void Initialize(float damage, float knockback, float speed) {
        _dmg = damage;
        _knockback = knockback;
        _speed = speed;
    }

    private void OnCollisionEnter(Collision collision) {
        FighterEntity entity = collision.gameObject.GetComponent<FighterEntity>();
        if (entity != null) {
            entity.ApplyDamage(_dmg, _knockback);
        }
        OnHit();
    }

    void OnHit() {
        Instantiate(ImpactVFX, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

}
