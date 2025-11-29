using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Private Data
    protected static GameManager _instance = null;
    private GridManager _map;
    #endregion

    #region Mono
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
    #endregion

    #region Public Method
    public static GameManager Instance { get => _instance; protected set => _instance = value; }
    public static GridManager GetMap()
    {
        return Instance._map;


    }
    #endregion
}
