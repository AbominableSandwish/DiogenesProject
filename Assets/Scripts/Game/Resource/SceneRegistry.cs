/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections.Generic;
using UnityEngine;
public class SceneRegistry : MonoBehaviour
{
    [SerializeField] public List<TextAsset> sceneNames;
    public static SceneRegistry Instance { get; private set; }

    private readonly Dictionary<string, string> _scenes = new();

    private void Awake()
    {
        sceneNames = new List<TextAsset>();

        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Clear() => _scenes.Clear();

    public void Register(TextAsset scene, string keyOverride = null)
    {
        if (scene == null) return;

        var key = string.IsNullOrWhiteSpace(keyOverride) ? scene.name : keyOverride;

        if (_scenes.ContainsKey(key))
            Debug.LogWarning($"TileRegistry: clé dupliquée '{key}' (remplacement).");

        sceneNames.Add(scene);
        _scenes[key] = scene.text;
    }

    public void RegisterMany(IEnumerable<TextAsset> scenes)
    {
        foreach (TextAsset s in scenes)
        {
            Register(s);
        }
    }

    public string Get(string key)
    {
        if (_scenes.TryGetValue(key, out var scene)) return scene;
        Debug.LogError($"TileRegistry: tile introuvable '{key}'");
        return null;
    }

    public bool TryGet(string key, out string scene) => _scenes.TryGetValue(key, out scene);
}
