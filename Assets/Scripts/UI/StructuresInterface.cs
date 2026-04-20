using UITKUtils;
using UnityEngine;
using UnityEngine.UIElements;

public class StructuresInterface : MonoBehaviour
{
    [SerializeField] private string name_visual = "structures";
    [SerializeField] private StructurePlacementController _structurePlacementController;

    private VisualElement _root;

    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        if (_root == null)
        {
            Debug.LogError("Missing root visual element.", this);
            return;
        }

        _structurePlacementController = UnityResolver.Resolve(_structurePlacementController, this, "TilemapPlacer");

        VisualElement visual = _root.Q<VisualElement>(name_visual);
        Validation.CheckQuery(visual, name_visual);

        if (visual == null)
            return;

        visual.style.flexDirection = FlexDirection.Row;

        int i = 0;
        VisualElement column = null;

        for (StructureType type = StructureType.Ground; type < StructureType.LENGTH; type++)
        {
            if (i == 0 || i % 4 == 0)
            {
                column = new VisualElement();
                column.name = "Structure_" + i;
                column.style.flexDirection = FlexDirection.Column;
                column.style.width = 80;
                column.style.height = 100;
                visual.Add(column);
            }

            StructureType capturedType = type;

            Button button = new Button();
            button.name = type + "_btn";
            button.style.width = 50;
            button.text = type.ToString();

            button.clicked += () => _structurePlacementController.SetSelectedType(capturedType);

            column.Add(button);
            i++;
        }
    }
}