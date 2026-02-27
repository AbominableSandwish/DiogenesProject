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
        visual.style.flexDirection = FlexDirection.Row;
        Validation.CheckQuery(visual, name_visual);

        int i = 0;
        VisualElement visualElement = null;
        for (StructureType type = StructureType.Coil; type < StructureType.LENGHT; type++)
        {

            if (i == 0 | i % 4 == 0)
            {
                visualElement = new VisualElement();
                visualElement.name = "Structure_" + i.ToString();          
                visualElement.style.flexDirection = FlexDirection.Column;
                visualElement.style.width = 60;
                visualElement.style.height = 100;
                visual.Add(visualElement);
            }

            var capturedType = type;

            Button button = new Button();
            button.name = type.ToString() + "_btn";
            button.style.width = 50;
            button.text = type.ToString();

            // TODO Register action
            button.clicked += () => NewStructure(capturedType) ;
            button.clicked += () => _placer.SetSelectedType(capturedType);

            visualElement.Add(button);
            i++;
        }

        _player = FindFirstObjectByType<PlayerController>();
       
    }

    private void NewStructure(StructureType type) {
         this._player.SelectStructure(type);
    }

}

