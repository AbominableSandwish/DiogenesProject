using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TilemapPlacer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap previewTilemap;

    [Header("Preview")]
    public TileBase previewTile;          // une tile "highlight" (ex: carré semi-transparent)
    public Color previewColor = new Color(1f, 1f, 1f, 0.5f);

    [SerializeField] private List<Tilemap> _tilemaps;

    [Header("Placement")]
    public bool blockIfOccupied = true;

    private Vector3Int lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
    private Structure.StructureType? selectedType;

    private PlayerController _player;

    private void Start()
    {
        previewTilemap = GetComponent<Tilemap>();
        _player = FindFirstObjectByType<PlayerController>();
    }

    void OnEnable()
    {
        if (previewTilemap != null)
        {
            previewTilemap.ClearAllTiles();
            previewTilemap.color = previewColor;
        }
    }

    void OnDisable()
    {
        if (previewTilemap != null)
            previewTilemap.ClearAllTiles();
    }

    // Appelé par tes boutons UI Toolkit
    public void SetSelectedType(Structure.StructureType type)
    {
        previewTile = TileRegistry.Instance.Get(type.ToString());
        selectedType = type;
    }

    void Update()
    {
        if (selectedType == null ||  previewTilemap == null)
            return;

        Vector3 world = GetMouseWorld2D();
        Vector3Int cell = previewTilemap.WorldToCell(world);

        // Preview: ne refaire que si la cellule change
        if (cell != lastCell)
        {
            previewTilemap.SetTile(lastCell, null);
            previewTilemap.SetTile(cell, previewTile);
            lastCell = cell;

            blockIfOccupied = false;
            foreach (Tilemap tilemap in _tilemaps)
            {
                blockIfOccupied = tilemap.GetTile(lastCell);

                if (blockIfOccupied)
                    break;
            }
            
        }

        // Placement
        if (Input.GetMouseButtonDown(0))
        {
            if (blockIfOccupied)
                return;

            // Ici tu peux soit placer une tile, soit instancier une "structure" (GameObject)
            PlaceStructure(selectedType.Value, cell);
        }

        // Annuler sélection au clic droit (optionnel)
        if (Input.GetMouseButtonDown(1))
        {
            selectedType = null;
            previewTilemap.SetTile(lastCell, null);
            lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
        }
    }

    Vector3 GetMouseWorld2D()
    {
        var cam = Camera.main;
        Vector3 mp = Input.mousePosition;
        mp.z = -cam.transform.position.z; // important en 2D ortho si caméra est à z=-10
        return cam.ScreenToWorldPoint(mp);
    }


    void PlaceStructure(Structure.StructureType type, Vector3Int cell)
    {
        // Exemple A: placer une tile correspondant au type
        // mainTilemap.SetTile(cell, GetTileForType(type));

        // Exemple B: logique de "structure" (données) + éventuellement un GameObject
        Debug.Log($"Place {type} sur {cell}");
        this._player.AddStructure(cell);
        // TODO: ton code:
        // - enregistrer la structure dans ton modèle (dictionnaire cell->structure)
        // - instancier un prefab si tu utilises des GameObjects
        // - ou poser une tile dédiée sur une tilemap "structures"
    }

    // TileBase GetTileForType(Structure.StructureType type) { ... }
}