using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawningSphereMovement : MonoBehaviour {

    public float Speed = 1f;
    public GameObject BallPrefab;

    public InputActionReference MoveInput;

    void Start() {
        MoveInput.action.Enable();
    }

    void Update() {
        InputHandling();
    }

    void InputHandling() {
        Vector2 move = MoveInput.action.ReadValue<Vector2>();

        transform.position = new Vector3(
                transform.position.x + (Speed * move.x * Time.deltaTime),
                transform.position.y,
                transform.position.z);

    }

    void SpawnBall() {
        Instantiate(BallPrefab, transform.position, Quaternion.identity);
    }

    // Korutina
    IEnumerator DestroyGameObjectAfterSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }

    // Zavolání
    // StartCoroutine(jmenoKorutiny(parametr));


}
