/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UITKUtils;
using UnityEngine;
using UnityEngine.UIElements;

class DebugMapEditor : MonoBehaviour
{

    [SerializeField] private MapManager mapManager;

    [SerializeField] private string _borderBtnName = "BorderBtn";

    private Button _borderBtn;
    bool isAlreadyGenerate = false;

    private void Awake()
    {
        mapManager = UnityResolver.Resolve(mapManager, this, "MapManager");

        // Root.
        VisualElement _root = GetComponent<UIDocument>().rootVisualElement;
        if (_root == null) { Debug.LogError("Missing references."); return; }

        _borderBtn = _root.Q<Button>(_borderBtnName);
        Validation.CheckQuery(_borderBtn, _borderBtnName);

        if (_borderBtn != null)
            _borderBtn.clicked += OnGenerateBorderClicked;
    }

    private void OnGenerateBorderClicked()
    {
        if (isAlreadyGenerate)
            return;

        int width = mapManager.Width;
        int height = mapManager.Height;

        for (int x = -1; x < width; x++)
        {
            for (int y = -1 ; y < height; y++)
            {
                if ((x == -1 || x == width - 1) || (y == -1 || y == height - 1))
                {
                    mapManager.BasicMap.structures[new Vector3Int(x, y, 0)] = new Limit();
                }
            }
        }
        mapManager.BasicMap.Refresh();

        isAlreadyGenerate = true;
    }
}
