/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections;

public interface ILoadStep
{
    string Name { get; }
    float Weight { get; } // importance relative dans la barre globale (ex: tiles=2, prefabs=3...)
    IEnumerator Execute(ILoadReporter reporter);
}

public interface ILoadReporter
{
    void SetStep(string stepName);
    void ReportItem(string itemLabel);
    void ReportProgress(float stepProgress01); // 0..1 pour la step en cours
    void Log(string line);
}