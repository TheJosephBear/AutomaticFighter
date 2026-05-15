using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterVFX : MonoBehaviour {

    public Vector3 effectOriginOffset;

    public List<GameObject> HitVFXPrefabs = new List<GameObject>();
    public GameObject BlockEffectPrefab;

    [Header("Pooling")]
    public int PoolSizePerPrefab = 5;

    // prefab -> pooled instances
    readonly Dictionary<GameObject, Queue<GameObject>> _pool = new();

    void Awake() {
        InitializePools();
    }

    public void PlayRandomHitEffect(float radius = 1f) {

        if (HitVFXPrefabs.Count == 0)
            return;

        GameObject prefab =
            HitVFXPrefabs[
                Random.Range(0, HitVFXPrefabs.Count)
            ];

        GameObject effect =
            GetFromPool(prefab);

        // random point inside sphere
        Vector3 randomOffset =
            Random.insideUnitSphere * radius;

        Vector3 spawnPosition =
            transform.position + effectOriginOffset + randomOffset;

        effect.transform.position = spawnPosition;

        effect.transform.rotation =
            Random.rotation;

        effect.SetActive(true);

        StartCoroutine(
            ReturnToPoolRoutine(
                prefab,
                effect,
                GetEffectLifetime(effect)
            )
        );
    }

    public void PlayBlockEffect() {
        Instantiate(BlockEffectPrefab, transform.position + effectOriginOffset, Quaternion.identity); 
    }

    #region Pooling

    void InitializePools() {

        foreach (GameObject prefab in HitVFXPrefabs) {

            if (prefab == null)
                continue;

            if (_pool.ContainsKey(prefab))
                continue;

            Queue<GameObject> queue =
                new Queue<GameObject>();

            for (int i = 0; i < PoolSizePerPrefab; i++) {

                GameObject instance =
                    Instantiate(prefab);

                instance.SetActive(false);

                queue.Enqueue(instance);
            }

            _pool.Add(prefab, queue);
        }
    }

    GameObject GetFromPool(GameObject prefab) {

        if (!_pool.ContainsKey(prefab)) {

            _pool.Add(
                prefab,
                new Queue<GameObject>()
            );
        }

        Queue<GameObject> queue =
            _pool[prefab];

        GameObject effect = null;

        // find inactive effect
        while (queue.Count > 0) {

            effect = queue.Dequeue();

            if (effect != null)
                break;
        }

        // pool empty -> create new
        if (effect == null) {

            effect = Instantiate(prefab);
        }

        return effect;
    }

    IEnumerator<System.Object> ReturnToPoolRoutine(
        GameObject prefab,
        GameObject effect,
        float delay
    ) {

        yield return new WaitForSeconds(delay);

        if (effect == null)
            yield break;

        effect.SetActive(false);

        if (!_pool.ContainsKey(prefab)) {

            _pool.Add(
                prefab,
                new Queue<GameObject>()
            );
        }

        _pool[prefab].Enqueue(effect);
    }

    float GetEffectLifetime(GameObject effect) {

        ParticleSystem particle =
            effect.GetComponentInChildren<ParticleSystem>();

        if (particle == null)
            return 2f;

        return particle.main.duration +
               particle.main.startLifetime.constantMax;
    }

    #endregion

}