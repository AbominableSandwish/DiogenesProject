/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadTilesAddressablesStep : ILoadStep
{
    public string Name => "Chargement des tiles";
    public float Weight => 2f;

    private readonly string _label;

    public LoadTilesAddressablesStep(string label = "Tile")
    {
        _label = label;
    }

    public IEnumerator Execute(ILoadReporter reporter)
    {
        if (TileRegistry.Instance == null)
        {
            reporter.Log("❌ TileRegistry.Instance est null (ajoute TileRegistry dans la scène Loading).");
            yield break;
        }

        TileRegistry.Instance.Clear();
        reporter.Log($"Label: {_label}");

        // On collecte pour éventuellement garder une liste locale (pas obligatoire)
        List<Tile> loaded = new();

        AsyncOperationHandle<IList<Tile>> handle = Addressables.LoadAssetsAsync<Tile>(
            _label,
            tile =>
            {
                if (tile == null) return;
                loaded.Add(tile);

                reporter.ReportItem($"Tile: {tile.name}");
                TileRegistry.Instance.Register(tile);
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