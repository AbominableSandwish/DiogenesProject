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
    public enum StructureLayer
    {
        Basic,
        Utility,
        Decoration
    }
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
    public Structure GetStructure(Vector3Int position, Structure.StructureLayer layer)
    {
        Structure structure = null;
        switch (layer)
        {
            case Structure.StructureLayer.Basic:
                structure = _basicMap?.GetStructure(position);
                break;  
            case Structure.StructureLayer.Utility:
                structure = _utilityMap?.GetStructure(position);
                break;
            case Structure.StructureLayer.Decoration:
                structure = _decorationMap?.GetStructure(position);
                break;
        }
        return structure;
    }
    public bool IsWalkable(Vector3Int position, Structure.StructureLayer layer)
    {
        bool isWalkable = false;
        switch (layer)
        {
            case Structure.StructureLayer.Basic:
                isWalkable = _basicMap.IsWalkable(position);
                break;
            case Structure.StructureLayer.Utility:
                isWalkable = _utilityMap.IsWalkable(position);
                break;
            case Structure.StructureLayer.Decoration:
                isWalkable = _decorationMap.IsWalkable(position);
                break;
        }
        return isWalkable;
    }


    public static bool AddStructure<T>(Vector3Int position)
    {
        MapManager grid = Instance;
        if (grid == null)
        {
            Debug.LogWarning("Map is null");
            return false;
        }

        string type = typeof(T).ToString();
        switch (type) {
            case "Ground":
                grid._basicMap.AddStructure<Ground>(position);
                break;
            case "WoodPlateform":
                grid._basicMap.AddStructure<WoodPlateform>(position);
                break;
            case "Limit":
                grid._basicMap.AddStructure<Limit>(position);
                break;
            case "Door":
                grid._basicMap.AddStructure<Door>(position);
                break;
            case "Glass":
                grid._basicMap.AddStructure<Glass>(position);
                break;
            case "Ladder":
                grid._basicMap.AddStructure<Ladder>(position);
                break;

            case "Coil":
                grid._utilityMap.AddStructure<Coil>(position);
                break;
            case "Generator":
                grid._utilityMap.AddStructure<Generator>(position);
                break;
            case "Engine":
                grid._utilityMap.AddStructure<Engine>(position);
                break;
            case "SolarPanel":
                grid._utilityMap.AddStructure<SolarPanel>(position);
                break;
            case "Lamp":
                grid._utilityMap.AddStructure<Lamp>(position);
                break;
        }

        return true;
    }

    public static bool RemoveStructure<T>(Vector3Int position)
    {
        MapManager grid = Instance;
        if (grid == null)
        {
            Debug.LogWarning("Map is null");
            return false;
        }

        switch (typeof(T).ToString())
        {
            case "Ground":
                grid._basicMap.RemoveStructure<WoodPlateform>(position);
                break;
            case "Door":
                grid._basicMap.RemoveStructure<Door>(position);
                break;
            case "Glass":
                grid._basicMap.RemoveStructure<Glass>(position);
                break;

            case "Coil":
                grid._utilityMap.RemoveStructure<Coil>(position);
                break;
            case "Generator":
                grid._utilityMap.RemoveStructure<Generator>(position);
                break;
            case "Engine":
                grid._utilityMap.RemoveStructure<Engine>(position);
                break;
            case "SolarPanel":
                grid._utilityMap.RemoveStructure<SolarPanel>(position);
                break;
            case "Lamp":
                grid._utilityMap.RemoveStructure<Lamp>(position);
                break;
        }

        return true;
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