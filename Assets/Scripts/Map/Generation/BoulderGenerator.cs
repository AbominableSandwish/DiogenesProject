/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Generation/Boulder Generator")]
public class BoulderGenerator : GenerationStep, IGenerationStep
{
    [Header("Cavity Shape")]
    [SerializeField] private Vector2Int center = new Vector2Int(10, 10);
    [SerializeField] private int radiusX = 6;
    [SerializeField] private int radiusY = 4;
    [SerializeField] private bool onlyUpperHalf = true;

    [SerializeField] private float noiseScale = 0.08f;
    [SerializeField] private float noiseStrength = 0.25f;

    public override IEnumerator DoGenerate(
     BasicMap map,
     int width,
     int height,
     int seed,
     IGenerationReporter reporter,
     int yieldEvery)
    {
        System.Random rng = new System.Random(seed);
        float offsetX = rng.Next(-100000, 100000);
        float offsetY = rng.Next(-100000, 100000);

        int totalCells = width * height;
        int processed = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float nx = (x - center.x) / (float)radiusX;
                float ny = (y - center.y) / (float)radiusY;

                float ellipse = nx * nx + ny * ny;

                bool validHalf = !onlyUpperHalf || y >= center.y;

                float noise = Mathf.PerlinNoise(
                    (x + offsetX) * noiseScale,
                    (y + offsetY) * noiseScale
                );

                float irregularity = (noise - 0.5f) * 2f * noiseStrength;

                if (validHalf && ellipse <= 1f + irregularity)
                {
                    map.RemoveStructure(new Vector3Int(x, y, 0));
                }

                processed++;

                if (processed % 50 == 0 || processed == totalCells)
                    reporter?.ReportProgress((float)processed / totalCells);
            }

            if (yieldEvery > 0 && x % yieldEvery == 0)
                yield return null;
        }

        map.Refresh();
        reporter?.ReportProgress(1f);
    }
}