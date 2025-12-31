using UITKUtils;
using UnityEngine;
using UnityEngine.UIElements;

class StructuresInterface : MonoBehaviour
{
    // Root.
    [SerializeField] private StyleSheet _button_css;
    [SerializeField] private string name_visual = "structures";

    private TilemapPlacer _placer;

    VisualElement _root;

    private PlayerController _player;

    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        if (_root == null) { Debug.LogError("Missing references."); return; }

        _placer = FindAnyObjectByType<TilemapPlacer>();

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
            button.clicked += () => _placer.SetSelectedType(capturedType);

            visual.Add(button);

        }

        _player = FindFirstObjectByType<PlayerController>();
    }

    private void NewStructure(Structure.StructureType type) {
         this._player.SelectStructure(type);
    }

}

