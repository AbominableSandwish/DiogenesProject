using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenerationMap : ScriptableObject
{
    [SerializeField] protected int seed = 1234;

    /// width/height viennent de ta "map" (pas du dictionnaire)
    public IEnumerator GenerateRoutine(
        BasicMap map,
        int width,
        int height,
        Action<float> onProgress,
        int yieldEvery = 1)
    {
        if (map == null)
        {
            Debug.LogError("map is null");
            yield break;
        }

        map.structures.Clear();
        onProgress?.Invoke(0f);

        yield return DoGenerateRoutine(map, width, height, onProgress, yieldEvery);

        onProgress?.Invoke(1f);
    }

    protected abstract IEnumerator DoGenerateRoutine(
        BasicMap map,
        int width,
        int height,
        Action<float> onProgress,
        int yieldEvery);
}

