/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;
using UnityEngine.UIElements;

public class TasksInterface : MonoBehaviour
{
    [SerializeField] private TaskSelectionController taskSelection;

    private VisualElement _taskMenu;
    private VisualElement _buildMenu;

    private void Start()
    {
        taskSelection = UnityResolver.Resolve(taskSelection, this, nameof(TaskSelectionController));

        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;

        _taskMenu = root.Q<VisualElement>("task-menu");
        _buildMenu = root.Q<VisualElement>("build-menu");

        if (_taskMenu == null || _buildMenu == null)
        {
            Debug.LogError("Missing task-menu or build-menu.", this);
            return;
        }

        BuildTaskMenu();
        BuildConstructionMenu();

        ShowTaskMenu();
    }

    private void BuildTaskMenu()
    {
        _taskMenu.Clear();

        Button buildButton = new Button(() =>
        {
            taskSelection.SelectTask(TaskType.Build);
            ShowBuildMenu();
        })
        {
            text = "Build"
        };

        Button gatherButton = new Button(() =>
        {
            taskSelection.SelectTask(TaskType.Gather);
        })
        {
            text = "Gather"
        };

        Button repairButton = new Button(() =>
        {
            taskSelection.SelectTask(TaskType.Repair);
        })
        {
            text = "Repair"
        };

        _taskMenu.Add(buildButton);
        _taskMenu.Add(gatherButton);
        _taskMenu.Add(repairButton);
    }

    private void BuildConstructionMenu()
    {
        _buildMenu.Clear();

        Button backButton = new Button(ShowTaskMenu)
        {
            text = "< Back"
        };

        _buildMenu.Add(backButton); 
        int i = 0;

        VisualElement currentColumn = null;

        for (StructureType type = StructureType.NONE + 1; type < StructureType.LENGTH; type++)
        {
            StructureType capturedType = type;

            if (i == 0 || i % 4 == 0)
            {
                currentColumn = new VisualElement();
                currentColumn.name = $"Structures_{i}";
                currentColumn.style.flexDirection = FlexDirection.Column;
                currentColumn.style.width = 80;
                currentColumn.style.height = 100;
                _buildMenu.Add(currentColumn);
            }


            Button button = new Button(() =>
            {
                taskSelection.SelectStructure(capturedType);
            })
            {
                name = $"{type}_btn",
                text = type.ToString()
            };
            button.style.width = 50;
            button.style.height = 30;

            currentColumn.Add(button);
            i++;
        }
    }

    private void ShowTaskMenu()
    {
        _taskMenu.style.display = DisplayStyle.Flex;
        _buildMenu.style.display = DisplayStyle.None;
    }

    private void ShowBuildMenu()
    {
        _taskMenu.style.display = DisplayStyle.None;
        _buildMenu.style.display = DisplayStyle.Flex;
    }
}