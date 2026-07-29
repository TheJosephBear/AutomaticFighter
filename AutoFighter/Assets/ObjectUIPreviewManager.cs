using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUIPreviewManager : MonoBehaviour {
    [Header("Preview Objects")]
    public List<PreviewObject> PreviewObjects = new List<PreviewObject>();

    [Header("Preview Settings")]
    [SerializeField] private int textureSize = 512;
    [SerializeField] private LayerMask previewLayer;
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0); // Transparent background

    [Header("Lighting Settings")]
    [SerializeField] private bool addPreviewLight = true;
    [SerializeField] private Color lightColor = Color.white;
    [SerializeField] private float lightIntensity = 1.2f;

    private Camera previewCamera;
    private Light previewLight;
    private RenderTexture renderTexture;

    private Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();
    private Dictionary<int, Texture2D> cachedTexturesByIndex = new Dictionary<int, Texture2D>();

    void Awake() {
        EnsureCameraExists();
    }

    void Start() {
        Setup();
    }

    void OnDestroy() {
        CleanupRenderTexture();
    }

    void Setup() {
        EnsureCameraExists();

        if (PreviewObjects.Count == 0) {
            foreach (Transform child in transform) {
                PreviewObject obj = new PreviewObject {
                    Name = child.name,
                    GameObject = child.gameObject
                };
                PreviewObjects.Add(obj);
            }
        }

        int layerIndex = GetFirstLayerFromMask(previewLayer);
        foreach (var obj in PreviewObjects) {
            if (obj.GameObject != null) {
                SetLayerRecursively(obj.GameObject, layerIndex);
            }
        }
    }

    private void EnsureCameraExists() {
        if (previewCamera != null && renderTexture != null && renderTexture.IsCreated()) {
            return;
        }

        Transform existingCam = transform.Find("Preview Camera");
        if (existingCam != null) {
            DestroyImmediate(existingCam.gameObject);
        }

        CreatePreviewCamera();
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
        previewCamera.nearClipPlane = 0.01f;
        previewCamera.farClipPlane = 100f;

        // Add optional directional light to ensure the character gets illuminated properly
        if (addPreviewLight) {
            GameObject lightObj = new GameObject("Preview Light");
            lightObj.transform.SetParent(camObj.transform);
            lightObj.transform.localRotation = Quaternion.Euler(30f, -30f, 0f);

            previewLight = lightObj.AddComponent<Light>();
            previewLight.type = LightType.Directional;
            previewLight.color = lightColor;
            previewLight.intensity = lightIntensity;
            previewLight.cullingMask = previewLayer;
        }

        CleanupRenderTexture();

        // Create RenderTexture with sRGB enabled (ReadWrite = Default) to fix blowout colors
        renderTexture = new RenderTexture(textureSize, textureSize, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB) {
            antiAliasing = 4 // Adds anti-aliasing to smooth character edges
        };
        renderTexture.Create();

        previewCamera.targetTexture = renderTexture;
    }

    private void CleanupRenderTexture() {
        if (renderTexture != null) {
            if (RenderTexture.active == renderTexture) {
                RenderTexture.active = null;
            }
            renderTexture.Release();
            Destroy(renderTexture);
            renderTexture = null;
        }
    }

    public Texture2D GetObjectPreviewTexture(int index) {
        EnsureCameraExists();

        if (cachedTexturesByIndex.TryGetValue(index, out Texture2D cached) && cached != null) {
            return cached;
        }

        if (index < 0 || index >= PreviewObjects.Count) {
            Debug.LogError($"Preview index {index} is invalid.");
            return null;
        }

        Texture2D tex = GeneratePreview(PreviewObjects[index]);
        cachedTexturesByIndex[index] = tex;

        return tex;
    }

    public Texture2D GetObjectPreviewTexture(string name) {
        EnsureCameraExists();

        if (cachedTextures.TryGetValue(name, out Texture2D cached) && cached != null) {
            return cached;
        }

        PreviewObject obj = PreviewObjects.Find(x => x.Name == name);
        if (obj == null) {
            Debug.LogError($"Preview object '{name}' not found.");
            return null;
        }

        Texture2D tex = GeneratePreview(obj);
        cachedTextures[name] = tex;

        return tex;
    }

    Texture2D GeneratePreview(PreviewObject previewObject) {
        if (previewObject.GameObject == null) {
            Debug.LogError("Preview object GameObject is null.");
            return null;
        }

        Bounds bounds = CalculateBounds(previewObject.GameObject);

        Vector3 center = bounds.center + previewObject.CameraLookAtOffset;
        Vector3 cameraPosition = center + previewObject.CameraPositionOffset;

        previewCamera.transform.position = cameraPosition;

        if (previewObject.CameraRotation != Vector3.zero) {
            previewCamera.transform.rotation = Quaternion.Euler(previewObject.CameraRotation);
        } else {
            previewCamera.transform.LookAt(center);
        }

        // Render the scene
        previewCamera.Render();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // Use sRGB linear space conversion matching Unity UI rendering
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false, false);
        texture.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0);
        texture.Apply();

        RenderTexture.active = currentRT;

        return texture;
    }

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
        if (layer < 0 || layer > 31) return;
        obj.layer = layer;

        foreach (Transform child in obj.transform) {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    int GetFirstLayerFromMask(LayerMask mask) {
        int bitmask = mask.value;
        if (bitmask == 0) return 0;

        for (int i = 0; i < 32; i++) {
            if ((bitmask & (1 << i)) != 0) {
                return i;
            }
        }
        return 0;
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