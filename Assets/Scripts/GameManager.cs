using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    protected static GameManager _instance = null;
    public static GameManager Instance { get => _instance; protected set => _instance = value; }

    [SerializeField] private Tilemap _ground;
    private Map _map;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        _map = new Map(_ground);
    }

    public static Map GetMap()
    {
        return Instance._map;
    }
}
