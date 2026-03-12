using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Generation/Voronoi Resource Generator")]
public class VoronoiResourceGenerator : GenerationStep
{
    [Header("Voronoi Settings")]
    [SerializeField] private int sitesPer100Cells = 6;

    [Header("Distribution")]
    [Range(0f, 1f)][SerializeField] private float earthPct = 0.30f;
    [Range(0f, 1f)][SerializeField] private float stonePct = 0.30f;
    [Range(0f, 1f)][SerializeField] private float rootsPct = 0.15f;
    [Range(0f, 1f)][SerializeField] private float waterPct = 0.05f;

    private enum ResourceKind { Earth, Stone, Roots, WaterSource }

    private struct Site
    {
        public Vector2 position;
        public ResourceKind kind;
    }

    public override IEnumerator DoGenerate(
        BasicMap map,
        int width,
        int height,
        int seed,
        IGenerationReporter reporter,
        int yieldEvery)
    {
        var rng = new System.Random(seed);

        // 1) Sites
        var sites = CreateSites(width, height, rng);


        // 2) Remplissage voronoi (coûteux) -> progress + yield
        int totalCells = width * height;
        int processed = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 cell = new Vector2(x, y);

                float bestDistance = float.MaxValue;
                ResourceKind bestKind = ResourceKind.Earth;

                for (int i = 0; i < sites.Count; i++)
                {
                    float d = (sites[i].position - cell).sqrMagnitude;
                    if (d < bestDistance)
                    {
                        bestDistance = d;
                        bestKind = sites[i].kind;
                    }
                }

                map.structures[new Vector3Int(x, y, 0)] = CreateResource(bestKind);

                processed++;

                reporter?.ReportProgress((float)processed / totalCells);
            }

            if (yieldEvery > 0 && (x % yieldEvery) == 0)
                yield return null;

        }

        map.Refresh();
    }

    private Structure CreateResource(ResourceKind kind)
    {
        // Ici tu instancies une NOUVELLE instance par cellule
        return kind switch
        {
            ResourceKind.Earth => new Earth(),
            ResourceKind.Stone => new Stone(),
            ResourceKind.Roots => new Roots(),
            ResourceKind.WaterSource => new WaterSource(),
            _ => new Earth()
        };
    }

    private List<Site> CreateSites(int width, int height, System.Random rng)
    {
        int totalCells = width * height;
        int siteCount = Mathf.Max(4, Mathf.RoundToInt(totalCells / 100f * sitesPer100Cells));

        NormalizePercentages(out float e, out float s, out float r, out float w);

        // calc un nb de sites par type (approx)
        int earthSites = Mathf.RoundToInt(siteCount * e);
        int stoneSites = Mathf.RoundToInt(siteCount * s);
        int rootsSites = Mathf.RoundToInt(siteCount * r);
        int waterSites = Mathf.Max(1, Mathf.RoundToInt(siteCount * w));

        // ajuste pour retomber sur siteCount
        int current = earthSites + stoneSites + rootsSites + waterSites;
        while (current < siteCount) { earthSites++; current++; }
        while (current > siteCount && earthSites > 0) { earthSites--; current--; }

        var sites = new List<Site>(siteCount);
        AddSites(sites, width, height, rng, ResourceKind.Earth, earthSites);
        AddSites(sites, width, height, rng, ResourceKind.Stone, stoneSites);
        AddSites(sites, width, height, rng, ResourceKind.Roots, rootsSites);
        AddSites(sites, width, height, rng, ResourceKind.WaterSource, waterSites);

        return sites;
    }

    private void NormalizePercentages(out float e, out float s, out float r, out float w)
    {
        float sum = earthPct + stonePct + rootsPct + waterPct;
        e = earthPct / sum;
        s = stonePct / sum;
        r = rootsPct / sum;
        w = waterPct / sum;
    }

    private void AddSites(List<Site> sites, int width, int height, System.Random rng, ResourceKind kind, int count)
    {
        for (int i = 0; i < count; i++)
        {
            // jitter léger pour casser la grille parfaite
            float x = (float)rng.NextDouble() * width + (float)rng.NextDouble() * 0.25f;
            float y = (float)rng.NextDouble() * height + (float)rng.NextDouble() * 0.25f;

            sites.Add(new Site { position = new Vector2(x, y), kind = kind });
        }
    }
}