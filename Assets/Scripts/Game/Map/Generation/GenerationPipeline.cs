using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Generation/Generation Pipeline")]
public class GenerationPipeline : ScriptableObject
{
    [SerializeField] private int seed = 1234;
    [SerializeField] private List<GenerationStep> steps = new();

    public int Seed => seed;
    public IReadOnlyList<GenerationStep> Steps => steps;
}