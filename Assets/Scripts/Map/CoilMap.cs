using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CoilMap : MonoBehaviour
{
    private static CoilMap _instance = null;
    private GameManager _game;
    private Map _map;
    private TileBase _zeroConnect;
    private List<TileBase> _oneConnect;
    private List<TileBase> _twoConnect;
    private List<TileBase> _threeConnect;
    private TileBase _fourConnect;
    private Tilemap _tilemap;
    private List<Circuit> _circuits;


    public static CoilMap Instance { get => _instance; protected set => _instance = value; }
    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        _game = GameManager.Instance;
        _map = GameManager.GetMap();
        _tilemap = GetComponent<Tilemap>();
        _circuits = new List<Circuit>();

        LoadSprite();
        //Generate();
    }

    #region Private methods
    private void Generate()
    {
        for (int i = -1; i <= _map.Height; i++)
        {
            for (int j = -1; j <= _map.Width; j++)
            {
                int rdm = UnityEngine.Random.Range(0, 2);

                if (rdm == 0)
                {
                    //nothing
                }

                if (rdm == 1)
                {
                    AddCoil(new Vector2Int(i, j));

                }
            }
        }
    }

    private void LoadSprite()
    {
        _zeroConnect = Resources.LoadAll<TileBase>("Sprite/Coils/0_connect")[0];

        _oneConnect = new List<TileBase>();
        foreach (TileBase tileBase in Resources.LoadAll<TileBase>("Sprite/Coils/1_connect"))
        {
            _oneConnect.Add(tileBase);
        }

        _twoConnect = new List<TileBase>();
        foreach (TileBase tileBase in Resources.LoadAll<TileBase>("Sprite/Coils/2_connect"))
        {
            _twoConnect.Add(tileBase);
        }

        _threeConnect = new List<TileBase>();
        foreach (TileBase tileBase in Resources.LoadAll<TileBase>("Sprite/Coils/3_connect"))
        {
            _threeConnect.Add(tileBase);
        }

        _fourConnect = Resources.LoadAll<TileBase>("Sprite/Coils/4_connect")[0];
    }

    private void RefreshTile(Vector2Int pos)
    {
        bool rightConnect = false;
        bool leftConnect = false;
        bool upConnect = false;
        bool downConnect = false;

        int connectCounter = 0;
        if ((pos.x > -1 || pos.x < _map.Height) || (pos.y > -1 || pos.y < _map.Width))
        {
            if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y)) != null)
            {

                TileBase tile = _zeroConnect;

                if (_tilemap.GetTile(new Vector3Int(pos.x + 1, pos.y)) != null)
                {
                    connectCounter++;
                    rightConnect = true;
                }

                if (_tilemap.GetTile(new Vector3Int(pos.x - 1, pos.y)) != null)
                {
                    connectCounter++;
                    leftConnect = true;
                }

                if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y + 1)) != null)
                {
                    connectCounter++;
                    upConnect = true;
                }

                if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y - 1)) != null)
                {
                    connectCounter++;
                    downConnect = true;
                }

                if (connectCounter == 1)
                {
                    if (leftConnect)
                    {
                        tile = _oneConnect[0];
                    }

                    if (upConnect)
                    {
                        tile = _oneConnect[1];
                    }

                    if (rightConnect)
                    {
                        tile = _oneConnect[2];
                    }

                    if (downConnect)
                    {
                        tile = _oneConnect[3];
                    }
                }

                if (connectCounter == 2)
                {
                    if (leftConnect && upConnect)
                    {
                        tile = _twoConnect[0];
                    }

                    if (leftConnect && rightConnect)
                    {
                        tile = _twoConnect[1];
                    }

                    if (leftConnect && downConnect)
                    {
                        tile = _twoConnect[2];
                    }

                    if (upConnect && downConnect)
                    {
                        tile = _twoConnect[3];
                    }

                    if (upConnect && rightConnect)
                    {
                        tile = _twoConnect[4];
                    }

                    if (rightConnect && downConnect)
                    {
                        tile = _twoConnect[5];
                    }

                }

                if (connectCounter == 3)
                {
                    if (leftConnect && upConnect && rightConnect)
                    {
                        tile = _threeConnect[0];
                    }

                    if (upConnect && rightConnect && downConnect)
                    {
                        tile = _threeConnect[1];
                    }

                    if (leftConnect && downConnect && rightConnect)
                    {
                        tile = _threeConnect[2];
                    }

                    if (downConnect && leftConnect && upConnect)
                    {
                        tile = _threeConnect[3];
                    }
                }

                if (connectCounter == 4)
                {
                    tile = _fourConnect;
                }

                _tilemap.SetTile(new Vector3Int(pos.x, pos.y), tile);
            }
        }
    }

    private List<TileBase> GetAllNeighboors(Vector2Int origin)
    {
        Queue<Vector3Int> toSearch = new Queue<Vector3Int>();
        List<TileBase> neighboors = new List<TileBase>();
        toSearch.Enqueue(new Vector3Int(origin.x, origin.y));

        while (toSearch.Count != 0)
        {
            Vector3Int pos = toSearch.Dequeue();
            if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.right) != null)
            {
                if (!neighboors.Contains(_tilemap.GetTile(pos + Vector3Int.right)))
                {
                    neighboors.Add(_tilemap.GetTile(pos + Vector3Int.right));
                    toSearch.Enqueue(pos + Vector3Int.right);
                }

            }
            if (_tilemap.GetTile(pos + Vector3Int.left) != null)
            {
                if (!neighboors.Contains(_tilemap.GetTile(pos + Vector3Int.left)))
                {
                    neighboors.Add(_tilemap.GetTile(pos + Vector3Int.left));
                    toSearch.Enqueue(pos + Vector3Int.left);
                }
            }
            if (_tilemap.GetTile(pos + Vector3Int.up) != null)
            {
                if (!neighboors.Contains(_tilemap.GetTile(pos + Vector3Int.up)))
                {
                    neighboors.Add(_tilemap.GetTile(pos + Vector3Int.up));
                    toSearch.Enqueue(pos + Vector3Int.up);
                }
            }
            if (_tilemap.GetTile(pos + Vector3Int.down) != null)
            {
                if (!neighboors.Contains(_tilemap.GetTile(pos + Vector3Int.down)))
                {
                    neighboors.Add(_tilemap.GetTile(pos + Vector3Int.down));
                    toSearch.Enqueue(pos + Vector3Int.down);
                }
            }
        }
        return neighboors;
    }
    #endregion

    #region Public methods
    public bool AddCoil(Vector2Int pos)
    {
        TileBase tile = null;
        List<TileBase> neighboors = new List<TileBase>();

        if ((pos.x > -1 || pos.x < _map.Height) || (pos.y > -1 || pos.y < _map.Width))
        {

            if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y)) != null) return false; // END


            //Self
            _tilemap.SetTile(new Vector3Int(pos.x, pos.y), _zeroConnect);
            RefreshTile(pos);
            tile = _tilemap.GetTile(new Vector3Int(pos.x, pos.y));
            //neighboor
            if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.right) != null)
            {
                neighboors.Add(_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.right));
                RefreshTile(pos + Vector2Int.right);
            }
            if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.left) != null)
            {
                neighboors.Add(_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.left));
                RefreshTile(pos + Vector2Int.left);
            }
            if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.up) != null)
            {
                neighboors.Add(_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.up));
                RefreshTile(pos + Vector2Int.up);
            }
            if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.down) != null)
            {
                neighboors.Add(_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.down));
                RefreshTile(pos + Vector2Int.down);
            }


            if (neighboors.Count == 0) return true; // END

            Queue<Circuit> targets = new Queue<Circuit>();
            foreach (Circuit circuit in _circuits)
            {
                foreach (TileBase neighboor in neighboors)
                {
                    if (!circuit.Contains(neighboor))
                    {
                        targets.Enqueue(circuit);
                        break;
                    }
                }
            }

            if(targets.Count == 0)
            {
                List<TileBase> path = new List<TileBase>();
                path.Add(tile);
                path.AddRange(neighboors);

                Circuit circuit = new Circuit(path);
                _circuits.Add(circuit);
            }
            else
            {
                Circuit newCircuit = new Circuit();
                newCircuit.AddTile(tile);
                while (targets.Count != 0)
                {
                    Circuit toMerge = targets.Dequeue();
                    _circuits.Remove(toMerge);
                    newCircuit.Merge(toMerge);
                }
                _circuits.Add(newCircuit);
            }
        }

        return true; // END
    }

    public bool RemoveCoil(Vector2Int pos)
    {
        bool canRemove = false;
        if ((pos.x > -1 || pos.x < _map.Height) || (pos.y > -1 || pos.y < _map.Width))
        {
            if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y)) != null)
            {
                _tilemap.SetTile(new Vector3Int(pos.x, pos.y), null);
                //Self
                RefreshTile(pos);
                //neihbor

                if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.right) != null)
                    RefreshTile(pos + Vector2Int.right);
                if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.left) != null)
                    RefreshTile(pos + Vector2Int.left);
                if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.up) != null)
                    RefreshTile(pos + Vector2Int.up);
                if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.down) != null)
                    RefreshTile(pos + Vector2Int.down);

                canRemove = true;
            }
        }
        return canRemove;
    }

    public static bool Add(Vector2Int pos)
    {
        return Instance.AddCoil(pos);
    }

    public static bool Remove(Vector2Int pos)
    {
        return Instance.RemoveCoil(pos);
    }
    #endregion

}
