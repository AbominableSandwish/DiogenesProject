using UnityEngine;
using UnityEngine.Tilemaps;
public class Map : MonoBehaviour
{
    #region Private Data
    //Self
    private static Map _instance = null;
    private int _height = 40;
    private int _width = 40;

    //Others
    [SerializeField] private BasicMap _basicMap;
    [SerializeField] private UtilityMap _utilityMap;
    [SerializeField] private DecorationMap _decorationMap;

    #endregion

    #region Public Data
    public static Map Instance { get => _instance; protected set => _instance = value; }
    public int Height { get => _height; }
    public int Width { get => _width; }
    #endregion

    #region Mono
    public void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
    }

    private void Start()
    {
    }
    #endregion

    #region Public Method
    public TileBase GetStructure(Vector2Int position, StructureType type)
    {
        TileBase tileBase = null;
        switch (type)
        {
            case StructureType.Basic:
                _basicMap?.GetStructure(new Vector2Int(position.x, position.y));
                break;  
            case StructureType.Utility:
                _utilityMap?.GetStructure(new Vector2Int(position.x, position.y));
                break;
            case StructureType.Decoration:
                _decorationMap?.GetStructure(new Vector2Int(position.x, position.y));
                break;
        }
        return tileBase;
    }

    public static bool AddStructure<T>(Vector2Int position)
    {
        Map map = Instance;
        if (map == null)
        {
            Debug.LogWarning("Map is null");
            return false;
        }

        switch (typeof(T).ToString()) {
            case "Ground":
                map._basicMap.AddStructure<Ground>(position);
                break;
            case "Door":
                map._basicMap.AddStructure<Door>(position);
                break;
            case "Wall":
                map._basicMap.AddStructure<Wall>(position);
                break;
            case "Glass":
                map._basicMap.AddStructure<Glass>(position);
                break;

            case "Coil":
                map._utilityMap.AddStructure<Coil>(position);
                break;
            case "Generator":
                map._utilityMap.AddStructure<Generator>(position);
                break;
            case "Engine":
                map._utilityMap.AddStructure<Engine>(position);
                break;
            case "SolarPanel":
                map._utilityMap.AddStructure<SolarPanel>(position);
                break;
            case "Lamp":
                map._utilityMap.AddStructure<Lamp>(position);
                break;
        }

        return true;
    }
    #endregion
}
