using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenerationStep : ScriptableObject, IGenerationStep
{
    [SerializeField] private string stepName = "Generation Step";
    [SerializeField] private float weight = 1f;

    public string Name => stepName;
    public float Weight => weight;
    /// width/height viennent de ta "map" (pas du dictionnaire)

    IEnumerator IGenerationStep.Generate(
        BasicMap map,
        int width,
        int height,
        int seed,
        IGenerationReporter reporter,
        int yieldEvery)
    {
        if (map == null)
        {
            Debug.LogError("map is null");
            yield break;
        }

        reporter?.SetStep(name);
        reporter?.ReportProgress(0f);

        yield return DoGenerate(map, width, height, seed, reporter, yieldEvery);

        reporter?.ReportProgress(1f);
    }

    public abstract IEnumerator DoGenerate(
        BasicMap map,
        int width,
        int height,
        int seed,
        IGenerationReporter reporter,
        int yieldEvery);
}