using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUIPreviewManager : MonoBehaviour {
    [Header("Preview Objects")]
    public List<PreviewObject> PreviewObjects = new List<PreviewObject>();

    [Header("Preview Settings")]
    [SerializeField] private int textureSize = 512;
    [SerializeField] private LayerMask previewLayer;
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0);

    private Camera previewCamera;
    private RenderTexture renderTexture;

    private Dictionary<string, Texture2D> cachedTextures =
        new Dictionary<string, Texture2D>();

    private Dictionary<int, Texture2D> cachedTexturesByIndex =
        new Dictionary<int, Texture2D>();

    void Awake() {
        Setup();
    }

    //========================================================
    // SETUP
    //========================================================

    void Setup() {
        CreatePreviewCamera();

        // Auto-fill preview objects from children if empty
        if (PreviewObjects.Count == 0) {
            foreach (Transform child in transform) {
                PreviewObject obj = new PreviewObject();
                obj.Name = child.name;
                obj.GameObject = child.gameObject;

                PreviewObjects.Add(obj);
            }
        }

        // Put all preview objects on preview layer
        foreach (var obj in PreviewObjects) {
            if (obj.GameObject != null) {
                SetLayerRecursively(obj.GameObject, Mathf.RoundToInt(Mathf.Log(previewLayer.value, 2)));
            }
        }
    }

    void CreatePreviewCamera() {
        GameObject camObj = new GameObject("Preview Camera");

        camObj.transform.SetParent(transform);

        previewCamera = camObj.AddComponent<Camera>();

        previewCamera.enabled = false;

        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor = backgroundColor;

        previewCamera.cullingMask = previewLayer;

        previewCamera.orthographic = false;

        renderTexture = new RenderTexture(textureSize, textureSize, 24, RenderTextureFormat.ARGB32);

        renderTexture.Create();

        previewCamera.targetTexture = renderTexture;
    }

    //========================================================
    // PUBLIC API
    //========================================================

    public Texture2D GetObjectPreviewTexture(int index) {
        if (cachedTexturesByIndex.ContainsKey(index))
            return cachedTexturesByIndex[index];

        if (index < 0 || index >= PreviewObjects.Count) {
            Debug.LogError($"Preview index {index} is invalid.");
            return null;
        }

        Texture2D tex = GeneratePreview(PreviewObjects[index]);

        cachedTexturesByIndex[index] = tex;

        return tex;
    }

    public Texture2D GetObjectPreviewTexture(string name) {
        if (cachedTextures.ContainsKey(name))
            return cachedTextures[name];

        PreviewObject obj = PreviewObjects.Find(x => x.Name == name);

        if (obj == null) {
            Debug.LogError($"Preview object '{name}' not found.");
            return null;
        }

        Texture2D tex = GeneratePreview(obj);

        cachedTextures[name] = tex;

        return tex;
    }

    //========================================================
    // GENERATE PREVIEW
    //========================================================

    Texture2D GeneratePreview(PreviewObject previewObject) {
        if (previewObject.GameObject == null) {
            Debug.LogError("Preview object GameObject is null.");
            return null;
        }

        Bounds bounds = CalculateBounds(previewObject.GameObject);

        Vector3 center =
            bounds.center + previewObject.CameraLookAtOffset;

        Vector3 cameraPosition =
            center + previewObject.CameraPositionOffset;

        previewCamera.transform.position = cameraPosition;

        if (previewObject.CameraRotation != Vector3.zero) {
            previewCamera.transform.rotation =
                Quaternion.Euler(previewObject.CameraRotation);
        } else {
            previewCamera.transform.LookAt(center);
        }

        previewCamera.Render();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D texture =
            new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);

        texture.ReadPixels(
            new Rect(0, 0, textureSize, textureSize),
            0,
            0);

        texture.Apply();

        RenderTexture.active = currentRT;

        return texture;
    }

    //========================================================
    // HELPERS
    //========================================================

    Bounds CalculateBounds(GameObject go) {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0) {
            return new Bounds(go.transform.position, Vector3.one);
        }

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer r in renderers) {
            bounds.Encapsulate(r.bounds);
        }

        return bounds;
    }

    void SetLayerRecursively(GameObject obj, int layer) {
        obj.layer = layer;

        foreach (Transform child in obj.transform) {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}

[System.Serializable]
public class PreviewObject {
    public string Name;

    public GameObject GameObject;

    public Vector3 CameraPositionOffset = new Vector3(0, 0, -3);

    public Vector3 CameraRotation;

    public Vector3 CameraLookAtOffset;
}