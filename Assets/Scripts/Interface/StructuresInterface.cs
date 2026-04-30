/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UITKUtils;
using UnityEngine;
using UnityEngine.UIElements;

public class StructuresInterface : MonoBehaviour
{
    [SerializeField] private string nameVisual = "structures";
    [SerializeField] private StructurePlacementController placer;


    private VisualElement root;

    private void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument missing.", this);
            return;
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("RootVisualElement missing.", this);
            return;
        }

        placer = UnityResolver.Resolve(placer, this, "StructurePlacementController");

        VisualElement visual = root.Q<VisualElement>(nameVisual);
        Validation.CheckQuery(visual, nameVisual);

        if (visual == null)
        {
            Debug.LogError($"VisualElement '{nameVisual}' not found.", this);
            return;
        }

        visual.style.flexDirection = FlexDirection.Row;

        int i = 0;
        VisualElement currentColumn = null;

        for (StructureType type = StructureType.Begin; type < StructureType.LENGTH; type++)
        {
            if (i == 0 || i % 4 == 0)
            {
                currentColumn = new VisualElement();
                currentColumn.name = $"Structure_{i}";
                currentColumn.style.flexDirection = FlexDirection.Column;
                currentColumn.style.width = 80;
                currentColumn.style.height = 100;
                visual.Add(currentColumn);
            }

            StructureType capturedType = type;

            Button button = new Button
            {
                name = $"{type}_btn",
                text = type.ToString()
            };

            button.style.width = 50;
            button.clicked += () => placer.SetSelectedType(capturedType);


            currentColumn.Add(button);
            i++;
        }
    }
}