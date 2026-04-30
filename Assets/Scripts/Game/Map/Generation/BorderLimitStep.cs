/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Generation/Border Limit Step")]
public class BorderLimitStep : GenerationStep, IGenerationStep
{
    public  override IEnumerator DoGenerate(
        BasicMap map,
        int width,
        int height,
        int seed,
        IGenerationReporter reporter,
        int yieldEvery)
    {

        // 2) Remplissage voronoi (coûteux) -> progress + yield
        int totalCells = width * height;
        int processed = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                processed++;
                if ((x == 0 || x == width - 1) || (y == 0 || y == height - 1)) {
                    map.structures[new Vector3Int(x, y, 0)] = new Limit();
                    reporter?.ReportProgress((float)processed / totalCells);
                }
            }

            if (yieldEvery > 0 && (x % yieldEvery) == 0)
                yield return null;
        }

        map.Refresh();
        reporter?.ReportProgress(1f);
    }
}