using System;
using UITKUtils;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

class StructuresInterface : MonoBehaviour
{
    // Root.
    [SerializeField] private StyleSheet _button_css;
    [SerializeField] private string name_visual = "structures";

    VisualElement _root;

    private PlayerController _player;

    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        if (_root == null) { Debug.LogError("Missing references."); return; }

        VisualElement visual = _root.Q<VisualElement>(name_visual);
        Validation.CheckQuery(visual, name_visual);

        for(Structure.StructureType type = Structure.StructureType.Coil; type < Structure.StructureType.LENGHT; type++)
        {
            var capturedType = type;

            Button button = new Button();
            button.name = type.ToString() + "_btn";
            button.style.width = 50;
            button.text = type.ToString();

            // TODO Register action
            button.clicked += () => NewStructure(capturedType) ;

            visual.Add(button);

        }

        _player = FindFirstObjectByType<PlayerController>();
    }

    private void NewStructure(Structure.StructureType type) {
         this._player.SelectStructure(type);
    }

}

