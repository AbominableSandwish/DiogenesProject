/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadScenesAddressablesStep : ILoadStep
{
    public string Name => "Chargement des tiles";
    public float Weight => 2f;

    private readonly string _label;

    public LoadScenesAddressablesStep(string label = "Scene")
    {
        _label = label;
    }

    public IEnumerator Execute(ILoadReporter reporter)
    {
        if (SceneRegistry.Instance == null)
        {
            reporter.Log("❌ SceneRegistry.Instance est null (ajoute TileRegistry dans la scène Loading).");
            yield break;
        }

       SceneRegistry.Instance.Clear();
        reporter.Log($"Label: {_label}");

        // On collecte pour éventuellement garder une liste locale (pas obligatoire)
        List<TextAsset> loaded = new();

        AsyncOperationHandle<IList<TextAsset>> handle = Addressables.LoadAssetsAsync<TextAsset>(
            _label,
            scene =>
            {
                if (scene == null) return;
                loaded.Add(scene);

                reporter.ReportItem($"Scene: {scene.name}");
                SceneRegistry.Instance.Register(scene);
            }
        );

        while (!handle.IsDone)
        {
            reporter.ReportProgress(handle.PercentComplete);
            yield return null;
        }

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            reporter.Log($"❌ Echec chargement tiles (label '{_label}').");
            yield break;
        }

        reporter.Log($"✅ Tiles chargées: {loaded.Count}");
        reporter.ReportProgress(1f);

        // IMPORTANT :
        // Ne pas Release(handle) ici si tu veux garder les tiles en mémoire.
        // Tu peux stocker le handle quelque part et le Release à la fermeture du jeu / retour menu principal.
    }
}