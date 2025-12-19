using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    #region Private Data
    //Self
    private static MapManager _instance = null;
    private int _height = 40;
    private int _width = 40;
    public float CellSize = 1f;

    //Others
    [SerializeField] private BasicMap _basicMap;
    [SerializeField] private UtilityMap _utilityMap;
    [SerializeField] private DecorationMap _decorationMap;
    #endregion

    #region Public Data
    public static MapManager Instance { get => _instance; protected set => _instance = value; }
    public int Height { get => _height; set => _height = value; }
    public int Width { get => _width; set => _width = value; }

    public Vector3 GetWorldPosition(Vector3Int gridPos) => new Vector3(gridPos.x, gridPos.y + gridPos.z * 0.5f, 0) * CellSize;

    public bool IsCellFree(Vector3Int gridPos)
        => _basicMap.GetStructure(gridPos) != null;
    #endregion

    #region Mono
    public void Awake()
    {
        Instance = this;
    }

    #endregion

    #region Public Method
    public Structure GetStructure(Vector3Int position, Structure.StructureMap map)
    {
        Structure structure = null;
        switch (map)
        {
            case Structure.StructureMap.Basic:
                structure = _basicMap?.GetStructure(position);
                break;  
            case Structure.StructureMap.Utility:
                structure = _utilityMap?.GetStructure(position);
                break;
            case Structure.StructureMap.Decoration:
                structure = _decorationMap?.GetStructure(position);
                break;
        }
        return structure;
    }
    public bool IsWalkable(Vector3Int position, Structure.StructureMap map)
    {
        bool isWalkable = false;
        switch (map)
        {
            case Structure.StructureMap.Basic:
                isWalkable = _basicMap.IsWalkable(position);
                break;
            case Structure.StructureMap.Utility:
                isWalkable = _utilityMap.IsWalkable(position);
                break;
            case Structure.StructureMap.Decoration:
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

        switch (typeof(T).ToString()) {
            case "Ground":
                grid._basicMap.AddStructure<Ground>(position);
                break;
            case "Door":
                grid._basicMap.AddStructure<Door>(position);
                break;
            case "Glass":
                grid._basicMap.AddStructure<Glass>(position);
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
                grid._basicMap.RemoveStructure<Ground>(position);
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
        string folder = Application.persistentDataPath;
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, fileName + ".json");
        File.WriteAllText(path, json);

        Debug.Log($"🌍 World.json loaded: {path}");
    }

    [ContextMenu("Load World (Single File)")]
    public void LoadWorld(string fileName = "World.json")
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"❌ Map not found : {path}");
            return;
        }

        string json = File.ReadAllText(path);
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
    }
    #endregion
}

public static class WorldStorage
{
    public static List<string> ListWorldFiles(string pattern = "*.json")
    {
        var list = new List<string>();
        var root = Application.persistentDataPath;
        if (!Directory.Exists(root)) return list;

        foreach (var path in Directory.GetFiles(root, pattern, SearchOption.TopDirectoryOnly))
            list.Add(Path.GetFileName(path));

        // Option : ne retourner que les fichiers qui contiennent un WorldData valide
        // (lecture rapide + try/catch)

        return list;
    }
}