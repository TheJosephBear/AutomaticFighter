using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterMovement : MonoBehaviour {

    public float RotationSpeed = 10f;
    public float CircleSpeedMultiplier = 0.8f;

    public float MinKnockbackStrength = 1f;
    public float MaxKnockbackStrength = 10f;

    Rigidbody _rb;

    int _circleDirection = 1;

    bool _knockback;

    void Awake() {

        _rb = GetComponent<Rigidbody>();
    }

    public void MoveToward(Vector3 position, float speed) {

        Vector3 dir =
            (position - transform.position).normalized;

        MoveInDirection(dir, speed);

        LookAt(position);
    }

    public void CircleAround(
        Vector3 target,
        float speed,
        float preferredDistance
    ) {

        Vector3 toEnemy =
            (target - transform.position).normalized;

        float distance =
            Vector3.Distance(transform.position, target);

        Vector3 correction = Vector3.zero;

        if (distance > preferredDistance + 0.5f)
            correction = toEnemy;

        else if (distance < preferredDistance - 0.5f)
            correction = -toEnemy;

        Vector3 side =
            Vector3.Cross(toEnemy, Vector3.up).normalized *
            _circleDirection;

        Vector3 move =
            (side + correction).normalized;

        MoveInDirection(
            move,
            speed * CircleSpeedMultiplier
        );

        LookAt(target);
    }

    public void MoveInDirection(
        Vector3 direction,
        float speed
    ) {

        if (_knockback)
            return;

        direction.y = 0f;

        Vector3 velocity =
            direction.normalized * speed;

        velocity.y = _rb.velocity.y;

        _rb.velocity = velocity;
    }

    public void StopMovement() {

        _rb.velocity = new Vector3(
            0f,
            _rb.velocity.y,
            0f
        );
    }

    public void LookAt(Vector3 targetPosition) {

        Vector3 dir =
            targetPosition - transform.position;

        dir.y = 0f;

        if (dir.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            RotationSpeed * Time.deltaTime
        );
    }

    public void ApplyKnockback(float strength) {

        float realStrength =
            Mathf.Clamp(strength, MinKnockbackStrength, MaxKnockbackStrength);

        Vector3 dir = -transform.forward;

        dir.y = 1f;

        _rb.AddForce(
            dir.normalized * realStrength,
            ForceMode.Impulse
        );
    }
}
