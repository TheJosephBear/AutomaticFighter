using UnityEngine;

public class BillboardUI : MonoBehaviour {
    private Transform mainCameraTransform;

    void Start() {
        // Cache the main camera's transform for performance
        if (Camera.main != null) {
            mainCameraTransform = Camera.main.transform;
        } else {
            Debug.LogError("BillboardUI: No Main Camera found in the scene! Please tag your camera as 'MainCamera'.");
        }
    }

    void LateUpdate() {
        if (mainCameraTransform != null) {
            // Makes the UI look directly at the camera
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                             mainCameraTransform.rotation * Vector3.up);
        }
    }
}