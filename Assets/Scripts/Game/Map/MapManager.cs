using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Structure;
public class MapManager : MonoBehaviour
{

    #region Private Data
    //Self
    private static MapManager _instance = null;
    [SerializeField] private int _height = 40;
    [SerializeField] private int _width = 40;
    //Others
    [SerializeField] private BasicMap _basicMap;
    [SerializeField] private UtilityMap _utilityMap;
    [SerializeField] private DecorationMap _decorationMap;

    [SerializeField] private bool GenerationMap = false;
    #endregion


    #region Public Data
    public static MapManager Instance { get => _instance; protected set => _instance = value; }
    public int Height { get => _height; set => _height = value; }
    public int Width { get => _width; set => _width = value; }

    public Vector3 GetWorldPosition(Vector3Int gridPos) => new Vector3(gridPos.x, gridPos.y + gridPos.z * 0.5f, 0) * CellSize;

    public bool IsCellFree(Vector3Int gridPos)
        => _basicMap.GetStructure(gridPos) != null;

    public float CellSize = 1f;

    public bool isReady = false;
    public Action OnExecute;
    #endregion

    #region Mono
    public void Awake()
    {
        Instance = this;
    }

    public void RecordAction()
    {
        
    }

    private void Start()
    {
        _basicMap?.Init(_width, _height, GenerationMap);
        _utilityMap?.Init(GenerationMap);
    }

    public void Execute()
    {
        OnExecute?.Invoke();
    }

    public void RegisterOnExecute(Action action)
    {
        OnExecute += action;
    }


    #endregion

    #region Public Method
    public Structure GetStructure(Vector3Int position, StructureLayer layer)
    {
        Structure structure = null;
        switch (layer)
        {
            case StructureLayer.Basic:
                structure = _basicMap?.GetStructure(position);
                break;  
            case StructureLayer.Utility:
                structure = _utilityMap?.GetStructure(position);
                break;
            case StructureLayer.Decoration:
                structure = _decorationMap?.GetStructure(position);
                break;
        }
        return structure;
    }
    public bool IsWalkable(Vector3Int position, StructureLayer layer)
    {
        bool isWalkable = false;
        switch (layer)
        {
            case StructureLayer.Basic:
                isWalkable = _basicMap.IsWalkable(position);
                break;
            case StructureLayer.Utility:
                isWalkable = _utilityMap.IsWalkable(position);
                break;
            case StructureLayer.Decoration:
                isWalkable = _decorationMap.IsWalkable(position);
                break;
        }
        return isWalkable;
    }


    public bool AddStructure(Structure structure, Vector3Int position)
    {
        if (structure == null)
        {
            Debug.LogError("Structure is null.", this);
            return false;
        }

        structure.Position = position;

        switch (structure.Layer)
        {
            case StructureLayer.Basic:
                _basicMap.AddStructure(structure, position);
                return true;

            case StructureLayer.Utility:
                _utilityMap.AddStructure(structure, position);
                return true;

            case StructureLayer.Decoration:
                _decorationMap.AddStructure(structure, position);
                return true;

            default:
                Debug.LogError($"Unsupported structure layer: {structure.Layer}", this);
                return false;
        }
    }

    public bool RemoveStructure(Structure structure, Vector3Int position)
    {
        if (structure == null)
        {
            Debug.LogError("Structure is null.", this);
            return false;
        }

        switch (structure.Layer)
        {
            case StructureLayer.Basic:
                return _basicMap.RemoveStructure(position);

            case StructureLayer.Utility:
                return _utilityMap.RemoveStructure(position);

            case StructureLayer.Decoration:
                return _decorationMap.RemoveStructure(position);

            default:
                Debug.LogError($"Unsupported structure layer: {structure.Layer}", this);
                return false;
        }
    }
    #endregion

    #region Serialization
    [System.Serializable]
    private class WorldSave
    {
        public string timestamp;
        public List<string> savedFiles = new();
    }

    [ContextMenu("Save World (Single File)")]
    public void SaveWorld(string fileName = "World")
    {
        var world = new WorldData
        {
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            width = this.Width,
            height = this.Height,
            maps = new List<NamedMapData>()
        };

        // Capture de chaque couche
        if (_basicMap != null)
            world.maps.Add(new NamedMapData { name = "Basic", data = _basicMap.Capture() });
        if (_utilityMap != null)
            world.maps.Add(new NamedMapData { name = "Utility", data = _utilityMap.Capture() });
        if (_decorationMap != null)
            world.maps.Add(new NamedMapData { name = "Decoration", data = _decorationMap.Capture() });

        // Serialize & write
        string json = JsonUtility.ToJson(world, true);
        FileSystem.WriteFile(Application.persistentDataPath, fileName, json);
    }

    [ContextMenu("Load World (Single File)")]
    public void LoadWorld(string fileName = "World.json")
    {
        isReady = false;
        string json = FileSystem.ReadFile(Path.Combine(Application.persistentDataPath, fileName));
        WorldData world = JsonUtility.FromJson<WorldData>(json);

        // Dimensions globales
        this.Width = world.width;
        this.Height = world.height;

        // Dispatch par nom
        foreach (var nm in world.maps)
        {
            switch (nm.name)
            {
                case "Basic":
                    if (_basicMap != null) _basicMap.Restore(nm.data);
                    break;
                case "Utility":
                    if (_utilityMap != null) _utilityMap.Restore(nm.data);
                    break;
                case "Decoration":
                    if (_decorationMap != null) _decorationMap.Restore(nm.data);
                    break;
                default:
                    Debug.LogWarning($"⚠️ Map inconnue dans World: {nm.name}");
                    break;
            }
        }

        Debug.Log($"✅ World Loaded: {fileName}");
        OnExecute?.Invoke();
        isReady = true;
    }

    [ContextMenu("Load World (Single File)")]
    public void LoadWorld(TextAsset asset)
    {
        isReady = false;
        string json = asset.text;
        WorldData world = JsonUtility.FromJson<WorldData>(json);

        // Dimensions globales
        this.Width = world.width;
        this.Height = world.height;

        // Dispatch par nom
        foreach (var nm in world.maps)
        {
            switch (nm.name)
            {
                case "Basic":
                    if (_basicMap != null) _basicMap.Restore(nm.data);
                    break;
                case "Utility":
                    if (_utilityMap != null) _utilityMap.Restore(nm.data);
                    break;
                case "Decoration":
                    if (_decorationMap != null) _decorationMap.Restore(nm.data);
                    break;
                default:
                    Debug.LogWarning($"⚠️ Map inconnue dans World: {nm.name}");
                    break;
            }
        }

        Debug.Log($"✅ World Loaded: {asset.name}");
        isReady = true;
        OnExecute?.Invoke();
    }
    #endregion

}

public static class WorldStorage
{
    public static List<string> ListWorldFiles(string pattern = "*.json")
    {
        return FileSystem.GetFiles(Application.persistentDataPath, pattern);
    }
}