using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class UtilityMap : StructureMap<UtilityMap>
{
    public enum UtilityType
    {
        Electricity,
        Gas,
        Liquid,
        Generator,
        Accumulator,
        Device
    }

    #region Private Data
    [SerializeField] private Tilemap _electric;
    [SerializeField] private Tilemap _liquid;
    [SerializeField] private Tilemap _gas;

    [SerializeField] private List<Circuit> _circuits;
    private int counterCoil = 0;
    private int counterGenerator = 0;
    private int counterStorage = 0;
    private int counterEngine = 0;
    //Coil
    private Sprite _zeroConnect;
    private List<Sprite> _oneConnect;
    private List<Sprite> _twoConnect;
    private List<Sprite> _threeConnect;
    private Sprite _fourConnect;
    //Solar Panel
    private TileBase _solarPanel;
    //Storage Battery
    private TileBase _storageBattery;
    //Storage Lamp
    private TileBase _lamp;

    #endregion

    #region Mono

    private void Start()
    {
        _game = GameManager.Instance;
        _map = Map.Instance;

        _circuits = new List<Circuit>();

        LoadSprite();
    }
    #endregion

    #region Private methods

    public void FixedUpdate()
    {
        foreach(Circuit circuit in _circuits)
        {
            circuit.Update();
        }
    }

    private void LoadSprite()
    {
        _zeroConnect = Resources.LoadAll<Sprite>("Sprite/Coils/0_connect")[0];

        _oneConnect = new List<Sprite>();
        foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprite/Coils/1_connect"))
        {
            _oneConnect.Add(sprite);
        }

        _twoConnect = new List<Sprite>();
        foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprite/Coils/2_connect"))
        {
            _twoConnect.Add(sprite);
        }

        _threeConnect = new List<Sprite>();
        foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprite/Coils/3_connect"))
        {
            _threeConnect.Add(sprite);
        }

        _fourConnect = Resources.LoadAll<Sprite>("Sprite/Coils/4_connect")[0];
        _solarPanel = Resources.LoadAll<TileBase>("Sprite/Generator/SolarPanel")[0];
        _lamp = Resources.LoadAll<TileBase>("Sprite/Engine/Lamp")[0];
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
            if (_electric.GetTile(new Vector3Int(pos.x, pos.y)) != null)
            {
                if (_electric.GetTile(new Vector3Int(pos.x, pos.y)) == null)
                    return;

                Sprite sprite = _zeroConnect;

                if (_electric.GetTile(new Vector3Int(pos.x + 1, pos.y)) != null  || _tilemap.GetTile(new Vector3Int(pos.x + 1, pos.y)) != null)
                {
                    connectCounter++;
                    rightConnect = true;
                }

                if (_electric.GetTile(new Vector3Int(pos.x - 1, pos.y)) != null || _tilemap.GetTile(new Vector3Int(pos.x - 1, pos.y)) != null)
                {
                    connectCounter++;
                    leftConnect = true;
                }

                if (_electric.GetTile(new Vector3Int(pos.x, pos.y + 1)) != null || _tilemap.GetTile(new Vector3Int(pos.x, pos.y + 1)) != null)
                {
                    connectCounter++;
                    upConnect = true;
                }

                if (_electric.GetTile(new Vector3Int(pos.x, pos.y - 1)) != null || _tilemap.GetTile(new Vector3Int(pos.x, pos.y - 1)) != null)
                {
                    connectCounter++;
                    downConnect = true;
                }

                if (connectCounter == 1)
                {
                    if (leftConnect)
                    {
                        sprite = _oneConnect[0];
                    }

                    if (upConnect)
                    {
                        sprite = _oneConnect[1];
                    }

                    if (rightConnect)
                    {
                        sprite = _oneConnect[2];
                    }

                    if (downConnect)
                    {
                        sprite = _oneConnect[3];
                    }
                }

                if (connectCounter == 2)
                {
                    if (leftConnect && upConnect)
                    {
                        sprite = _twoConnect[0];
                    }

                    if (leftConnect && rightConnect)
                    {
                        sprite = _twoConnect[1];
                    }

                    if (leftConnect && downConnect)
                    {
                        sprite = _twoConnect[2];
                    }

                    if (upConnect && downConnect)
                    {
                        sprite = _twoConnect[3];
                    }

                    if (upConnect && rightConnect)
                    {
                        sprite = _twoConnect[4];
                    }

                    if (rightConnect && downConnect)
                    {
                        sprite = _twoConnect[5];
                    }

                }

                if (connectCounter == 3)
                {
                    if (leftConnect && upConnect && rightConnect)
                    {
                        sprite = _threeConnect[0];
                    }

                    if (upConnect && rightConnect && downConnect)
                    {
                        sprite = _threeConnect[1];
                    }

                    if (leftConnect && downConnect && rightConnect)
                    {
                        sprite = _threeConnect[2];
                    }

                    if (downConnect && leftConnect && upConnect)
                    {
                        sprite = _threeConnect[3];
                    }
                }

                if (connectCounter == 4)
                {
                    sprite = _fourConnect;
                }

                //Tile tile = new Tile();
                //tile.name = ((Tile)_electric.GetTile(new Vector3Int(pos.x, pos.y))).name;
                Tile tile = (Tile)_electric.GetTile(new Vector3Int(pos.x, pos.y));
                tile.sprite = sprite;
                tile.colliderType = Tile.ColliderType.Grid;
                _electric.SetTile(new Vector3Int(pos.x, pos.y), tile);
                _electric.RefreshTile(new Vector3Int(pos.x, pos.y));
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

    public override bool AddStructure<T>(Vector2Int pos)
    {
        bool canAdd = false;
        switch (typeof(T))
        {
            case var cls when cls == typeof(Coil):
                AddCoil(pos);
                break;
            case var cls when cls == typeof(SolarPanel):
                AddGenerator<T>(pos);
                break;
            case var cls when cls == typeof(Lamp):
                AddEngine<Lamp>(pos);
                break;
        }
        return canAdd;
    }
    public bool AddEngine<T>(Vector2Int pos)
    {
        Tile tile = null;
        List<Tile> neighboors = new List<Tile>();

        if ((pos.x > -1 || pos.x < _map.Height) || (pos.y > -1 || pos.y < _map.Width))
        {

            if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y)) != null) return false; // END



            TileBase tileBase = null;
            switch (typeof(T))
            {
                case var cls when cls == typeof(Lamp):
                    tileBase = _lamp;
                    break;
            }

            //Self
            _tilemap.SetTile(new Vector3Int(pos.x, pos.y), tileBase);
            tile = (Tile)_tilemap.GetTile(new Vector3Int(pos.x, pos.y));
            tile.name = typeof(T) + "_" + counterEngine;
            //neighboor
            if (_electric.GetTile(new Vector3Int(pos.x + 1, pos.y)) != null)
            {
                RefreshTile(pos + Vector2Int.right);
                neighboors.Add((Tile)_electric.GetTile(new Vector3Int(pos.x + 1, pos.y)));
            }
            if (_electric.GetTile(new Vector3Int(pos.x - 1, pos.y)) != null)
            {
                RefreshTile(pos + Vector2Int.left);
                neighboors.Add((Tile)_electric.GetTile(new Vector3Int(pos.x - 1, pos.y)));
            }
            if (_electric.GetTile(new Vector3Int(pos.x, pos.y + 1)) != null)
            {
                RefreshTile(pos + Vector2Int.up);
                neighboors.Add((Tile)_electric.GetTile(new Vector3Int(pos.x, pos.y + 1)));
            }
            if (_electric.GetTile(new Vector3Int(pos.x, pos.y - 1)) != null)
            {
                RefreshTile(pos + Vector2Int.down);
                neighboors.Add((Tile)_electric.GetTile(new Vector3Int(pos.x, pos.y - 1)));
            }
        
            object[] args = { _tilemap, pos.x, pos.y };
            object instance = typeof(T).Instantiate(true, args);
            Engine engine = (Engine)instance;

            counterEngine++;

            if (neighboors.Count == 0) return true; // END

            Queue<Circuit> targets = new Queue<Circuit>();
            foreach (Circuit circuit in _circuits)
            {
                foreach (Tile neighboor in neighboors)
                {
                    if (circuit.Contains(neighboor))
                    {
                        targets.Enqueue(circuit);
                        break;
                    }
                }
            }

            if (targets.Count == 0)
            {
                List<TileBase> path = new List<TileBase>();
                path.AddRange(neighboors);

                Circuit circuit = new Circuit(path);
                circuit.AddEngine(engine);
                _circuits.Add(circuit);
            }
            else
            {
                Circuit newCircuit = new Circuit();
               
                newCircuit.AddEngine(engine);
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

    public bool AddGenerator<T>(Vector2Int pos)
    {

        TileBase tile = null;

        List<TileBase> neighboors = new List<TileBase>();

        if ((pos.x > -1 || pos.x < _map.Height) || (pos.y > -1 || pos.y < _map.Width))
        {

            if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y)) != null) return false; // END

            counterGenerator++;

            //Self
            _tilemap.SetTile(new Vector3Int(pos.x, pos.y), _solarPanel);
            tile = _tilemap.GetTile(new Vector3Int(pos.x, pos.y));
            tile.name = typeof(T) + counterGenerator.ToString();
            //neighboor
            if (_electric.GetTile(new Vector3Int(pos.x + 1, pos.y)) != null)
            {
                neighboors.Add(_electric.GetTile(new Vector3Int(pos.x + 1, pos.y)));
                RefreshTile(pos + Vector2Int.right);
            }
            if (_electric.GetTile(new Vector3Int(pos.x - 1, pos.y)) != null)
            {
                neighboors.Add(_electric.GetTile(new Vector3Int(pos.x - 1, pos.y)));
                RefreshTile(pos + Vector2Int.left);
            }
            if (_electric.GetTile(new Vector3Int(pos.x, pos.y + 1)) != null)
            {
                neighboors.Add(_electric.GetTile(new Vector3Int(pos.x, pos.y + 1)));
                RefreshTile(pos + Vector2Int.up);
            }
            if (_electric.GetTile(new Vector3Int(pos.x, pos.y - 1)) != null)
            {
                neighboors.Add(_electric.GetTile(new Vector3Int(pos.x, pos.y - 1)));
                RefreshTile(pos + Vector2Int.down);
            }


            if (neighboors.Count == 0) return true; // END

            Queue<Circuit> targets = new Queue<Circuit>();
            foreach (Circuit circuit in _circuits)
            {
                foreach (Tile neighboor in neighboors)
                {
                    if (circuit.Contains(neighboor))
                    {
                        targets.Enqueue(circuit);
                        break;
                    }
                }
            }

            if (targets.Count == 0)
            {
                List<TileBase> path = new List<TileBase>();
                path.AddRange(neighboors);

                Circuit circuit = new Circuit(path);
                circuit.AddGenerator((Generator)typeof(T).Instantiate());
                _circuits.Add(circuit);
            }
            else
            {
                Circuit newCircuit = new Circuit();
                newCircuit.AddGenerator((Generator)typeof(T).Instantiate());
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

    public bool AddCoil(Vector2Int pos)
    {
        Tile tile = null;

        List<Tile> neighboors = new List<Tile>();

        if ((pos.x > -1 || pos.x < _map.Height) || (pos.y > -1 || pos.y < _map.Width))
        {

            if (_electric.GetTile(new Vector3Int(pos.x, pos.y)) != null) return false; // END

            //Self
            tile = new Tile();
            tile.name = "Coil_" + counterCoil.ToString();
            tile.sprite = _zeroConnect;
            tile.colliderType = Tile.ColliderType.Grid;
            _electric.SetTile(new Vector3Int(pos.x, pos.y), tile);

            RefreshTile(pos);

           
            counterCoil++;
           
            //neighboor
            if (_electric.GetTile(new Vector3Int(pos.x + 1, pos.y)) != null)
            {
                neighboors.Add((Tile)_electric.GetTile(new Vector3Int(pos.x + 1, pos.y)));
                RefreshTile(pos + Vector2Int.right);
            }
            if (_electric.GetTile(new Vector3Int(pos.x - 1, pos.y)) != null)
            {
                neighboors.Add((Tile)_electric.GetTile(new Vector3Int(pos.x - 1, pos.y)));
                RefreshTile(pos + Vector2Int.left);
            }
            if (_electric.GetTile(new Vector3Int(pos.x, pos.y + 1)) != null)
            {
                neighboors.Add((Tile)_electric.GetTile(new Vector3Int(pos.x, pos.y + 1)));
                RefreshTile(pos + Vector2Int.up);
            }
            if (_electric.GetTile(new Vector3Int(pos.x, pos.y - 1)) != null)
            {
                neighboors.Add((Tile)_electric.GetTile(new Vector3Int(pos.x, pos.y - 1)));
                RefreshTile(pos + Vector2Int.down);
            }


            if (neighboors.Count == 0) return true; // END

            Queue<Circuit> targets = new Queue<Circuit>();
            foreach (Circuit circuit in _circuits)
            {
                foreach (Tile neighboor in neighboors)
                {
                    if (circuit.Contains(neighboor))
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
            if (_electric.GetTile(new Vector3Int(pos.x, pos.y)) != null)
            {
                _electric.SetTile(new Vector3Int(pos.x, pos.y), null);
                //Self
                RefreshTile(pos);
                //neihbor

                if (_electric.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.right) != null)
                    RefreshTile(pos + Vector2Int.right);
                if (_electric.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.left) != null)
                    RefreshTile(pos + Vector2Int.left);
                if (_electric.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.up) != null)
                    RefreshTile(pos + Vector2Int.up);
                if (_electric.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.down) != null)
                    RefreshTile(pos + Vector2Int.down);

                canRemove = true;
            }
        }
        return canRemove;
    }

    public bool RemoveStructure(Vector2Int pos, UtilityType type)
    {
        bool canRemove = false;
        switch (type)
        {
            case UtilityType.Electricity:
                canRemove = RemoveCoil(pos);
                break;
        }
        return canRemove;
    }
    #endregion

}
