using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Generation/Empty Cavity Generator")]
public class EmptyCavityGenerator : GenerationStep, IGenerationStep
{
    [Header("Cavity Shape")]
    [SerializeField] private Vector2Int center = new Vector2Int(10, 10);
    [SerializeField] private int radiusX = 6;
    [SerializeField] private int radiusY = 4;
    [SerializeField] private bool onlyUpperHalf = true;

    public override IEnumerator DoGenerate(
        BasicMap map,
        int width,
        int height,
        int seed,
        IGenerationReporter reporter,
        int yieldEvery)
    {
        int totalCells = width * height;
        int processed = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float nx = (x - center.x) / (float)radiusX;
                float ny = (y - center.y) / (float)radiusY;

                bool insideEllipse = (nx * nx + ny * ny) <= 1f;
                bool validHalf = !onlyUpperHalf || y >= center.y;

                if (insideEllipse && validHalf)
                {
                    map.RemoveStructure(new Vector3Int(x, y, 0));
                }

                processed++;

                if (processed % 25 == 0)
                    reporter?.ReportProgress((float)processed / totalCells);
            }

            if (yieldEvery > 0 && x % yieldEvery == 0)
                yield return null;
        }

        map.Refresh();
        reporter?.ReportProgress(1f);
    }
}