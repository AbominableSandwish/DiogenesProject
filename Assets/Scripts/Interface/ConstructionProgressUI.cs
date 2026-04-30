/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;
using UnityEngine.UIElements;

public class ConstructionProgressUI : MonoBehaviour
{
    [SerializeField] private UIDocument document;

    private VisualElement root;
    private VisualElement progressFill;
    private Label progressLabel;

    private ConstructionSite currentSite;

    private void Awake()
    {
        root = document.rootVisualElement;

        progressFill = root.Q<VisualElement>("construction-progress-fill");
        progressLabel = root.Q<Label>("construction-progress-label");

        Hide();
    }

    private void Update()
    {
        if (currentSite == null)
            return;

        float progress = currentSite.Progress;

        progressFill.style.width = Length.Percent(progress * 100f);
        progressLabel.text = $"{Mathf.RoundToInt(progress * 100f)}%";

        if (currentSite.IsCompleted || currentSite == null)
        {
            Hide();
            Destroy(this.gameObject);
        }
           
    }

    public void Show(ConstructionSite site)
    {
        currentSite = site;
        root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        currentSite = null;
        root.style.display = DisplayStyle.None;

    }
}