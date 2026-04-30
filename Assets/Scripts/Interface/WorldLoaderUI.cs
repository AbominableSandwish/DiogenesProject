/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldLoaderUI : MonoBehaviour
{
    [SerializeField] private UIDocument _ui;
    private ListView _list;
    private Button _load;

    private List<string> _files;

    [SerializeField] private string name_ListView = "world_list";
    [SerializeField] private string name_BtnLoad = "load_btn";

    private void Start()
    {
        var root = _ui.rootVisualElement;
        _list = root.Q<ListView>(name_ListView);
        _load = root.Q<Button>(name_BtnLoad);

        _files = WorldStorage.ListWorldFiles("World*.json"); // ou "*.json"
        _list.itemsSource = _files;
        _list.makeItem = () => new Label();
        _list.bindItem = (e, i) => (e as Label).text = _files[i];

        _load.clicked += () =>
        {
            if (_list.selectedIndex < 0) { Debug.LogWarning("Aucun monde sélectionné."); return; }
            string file = _files[_list.selectedIndex];
            MapManager.Instance.LoadWorld(file);
        };
    }
}