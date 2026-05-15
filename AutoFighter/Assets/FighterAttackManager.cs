using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAttackManager : MonoBehaviour {

    public FighterAttackBase BasicAttack;
    public List<FighterAttackBase> AvailableAttacks = new List<FighterAttackBase>();

    public float CastBasicAttack() {
        return BasicAttack.ExecuteAttack();
    }

    public float CastRandomAttack() {
        if(AvailableAttacks.Count == 0) {
            print("No special attack to cast!");
            return 0f;
        }
        return AvailableAttacks[Random.Range(0, AvailableAttacks.Count)].ExecuteAttack();
    }
}
