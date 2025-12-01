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
            case "Wall":
                grid._basicMap.AddStructure<Wall>(position);
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
            case "Wall":
                grid._basicMap.RemoveStructure<Wall>(position);
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

    [ContextMenu("Save All Maps")]
    public void SaveAllMaps()
    {
        // 1) Sauver chaque map via leur propre méthode (qui sauve dans persistentDataPath)
        //_basicMap.SaveMap();       // produit "BasicMap.json"
        _utilityMap.SaveMap();     // produit "UtilityMap.json"
       // _decorationMap.SaveMap();  // produit "DecorationMap.json"

        // 2) Composer le sommaire World.json
        var world = new WorldSave
        {
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            savedFiles = new System.Collections.Generic.List<string>
            {
                "BasicMap.json",
                "UtilityMap.json",
                "DecorationMap.json"
            }
        };

        string json = JsonUtility.ToJson(world, true);

        string folder = Application.persistentDataPath;
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, "World.json");
        File.WriteAllText(path, json);

        Debug.Log($"🌍 World.json loaded: {path}");
    }

    public void LoadMapFromPersistent(string fileName)
{
    string fullPath = Path.Combine(Application.persistentDataPath, fileName);

    if (!File.Exists(fullPath))
    {
        Debug.LogError($"❌ Map not found : {fullPath}");
        return;
    }

    string json = File.ReadAllText(fullPath);
    MapData mapData = JsonUtility.FromJson<MapData>(json);

    // Applique les dimensions
    this.Width = mapData.width;
    this.Height = mapData.height;

    // Recharge les cellules
    foreach (MapCellData cell in mapData.cells)
    {
        Vector3Int pos = new Vector3Int(cell.x, cell.y, cell.z);
        switch (cell.type)
        {
            case "Ground":
                MapManager.AddStructure<Ground>(pos);
                break;
            case "Ladder":
                MapManager.AddStructure<Ladder>(pos);
                break;
            // case "Stair": GridManager.AddStructure<Stair>(pos); break;
        }
    }

    Debug.Log($"✅ Map {fileName} Loaded ({mapData.cells.Count} cells) from persistentDataPath !");
}
    #endregion
}
