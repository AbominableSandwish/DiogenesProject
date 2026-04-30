/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

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