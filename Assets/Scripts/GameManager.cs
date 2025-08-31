using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    protected static GameManager _instance = null;

    private Map _map;

    public static GameManager Instance { get => _instance; protected set => _instance = value; }
  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public static Map GetMap()
    {
        return Instance._map;
    }
}
