using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRegistry : MonoBehaviour
{
    public static TileRegistry Instance { get; private set; }

    private readonly Dictionary<string, Tile> _tiles = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Clear() => _tiles.Clear();

    public void Register(Tile tile, string keyOverride = null)
    {
        if (tile == null) return;

        var key = string.IsNullOrWhiteSpace(keyOverride) ? tile.name : keyOverride;

        if (_tiles.ContainsKey(key))
            Debug.LogWarning($"TileRegistry: clé dupliquée '{key}' (remplacement).");

        _tiles[key] = tile;
    }

    public void RegisterMany(IEnumerable<Tile> tiles)
    {
        int i = 0;
        foreach (Tile t in tiles)
        {
            Tilemap tilemap = GetComponent<Tilemap>();
            tilemap.SetTile(Vector3Int.right * i + Vector3Int.down * 2, t);
            Register(t);
            i++;
        }
    }

    public Tile Get(string key)
    {
        if (_tiles.TryGetValue(key, out var tile)) return tile;
        Debug.LogError($"TileRegistry: tile introuvable '{key}'");
        return null;
    }

    public bool TryGet(string key, out Tile tile) => _tiles.TryGetValue(key, out tile);
}
