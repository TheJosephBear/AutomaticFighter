using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerControl : MonoBehaviour {


    void Start() {

    }

    void Update() {

    }


    private void OnTriggerEnter(Collider other) {
        // Je to ta refferencnutá koule?
        if (other.CompareTag("ball")) {
            Debug.Log("Trefa!");
        }
    }

}
