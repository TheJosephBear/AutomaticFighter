using UnityEngine;

public class BasicAttack : FighterAttackBase {

    [Header("Hit Scan")]
    public Vector3 BoxSize = new Vector3(1.5f, 1f, 2f);

    public Vector3 BoxOffset = new Vector3(0f, 1f, 1f);

    public LayerMask HitLayers = ~0;

    public Color GizmoColor = Color.red;

    public override float ExecuteAttack() {

        CurrentDamage = BaseDamage + _entity.Damage.CurrentValue;

        ScanAndDamage();
        return GetAttackDuration();
    }

    void ScanAndDamage() {

        Vector3 center = GetBoxCenter();

        Collider[] hits = Physics.OverlapBox(
            center,
            BoxSize * 0.5f,
            transform.rotation,
            HitLayers
        );

        foreach (Collider hit in hits) {

            FighterEntity fighter =
                hit.GetComponent<FighterEntity>();

            if (fighter == null)
                continue;

            // Don't hit self
            if (fighter == _entity)
                continue;

            fighter.ApplyDamage(CurrentDamage, KnockbackStrength);
        }
    }

    Vector3 GetBoxCenter() {

        return transform.position +
               transform.rotation * BoxOffset;
    }

    void OnDrawGizmosSelected() {

        Gizmos.color = GizmoColor;

        Matrix4x4 oldMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(
            GetBoxCenter(),
            transform.rotation,
            Vector3.one
        );

        Gizmos.DrawWireCube(
            Vector3.zero,
            BoxSize
        );

        Gizmos.matrix = oldMatrix;
    }
}