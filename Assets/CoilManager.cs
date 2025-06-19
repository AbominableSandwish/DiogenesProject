using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class CoilManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameManager _game;
    private Map _map;
    private TileBase _zeroConnect;
    private List<TileBase> _oneConnect;
    private List<TileBase> _twoConnect;
    private List<TileBase> _threeConnect;
    private TileBase _fourConnect;

    private Tilemap _tilemap;
    void Start()
    {
        _game = GameManager.Instance;
        _tilemap = GetComponent<Tilemap>();

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

        _map = GameManager.GetMap();
        for (int i = -1; i <= _map.Height; i++)
        {
            for (int j = -1; j <= _map.Width; j++)
            {
                int rdm = Random.Range(0, 2);

                if (rdm == 0)
                {
                    //nothing
                }

                if(rdm == 1)
                {
                    AddCoil(new Vector2Int(i, j));
                    
                }

                if ((i < 0 || i > _map.Height - 1) || (j < 0 || j > _map.Width - 1))
                {

                }
            }
        }

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
                if(leftConnect && upConnect)
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

    public void AddCoil(Vector2Int pos)
    {
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

    }

    public void RemoveCoil()
    {

    }
}
