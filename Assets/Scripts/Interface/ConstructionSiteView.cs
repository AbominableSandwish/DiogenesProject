/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ConstructionSiteView : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float heightOffset = 0.8f;

    private VisualElement root;
    private VisualElement progressfill;
    private VisualElement progressBackground;
    private VisualElement progressShine;
    private Label label;

    MapManager mapManager;
    private ConstructionSite site;
    private Vector3Int cellPosition;
    private Tilemap tilemap;
    private bool _isVisible = false;

    private void Awake()
    {
        root = document.rootVisualElement;
        progressfill = root.Q<VisualElement>("construction-progress-fill");      
        progressShine = root.Q<VisualElement>("construction-progress-shine");
        progressBackground = root.Q<VisualElement>("construction-progress-background");
        label = root.Q<Label>("construction-progress-label");

        if (targetCamera == null)
            targetCamera = Camera.main;

        root.style.display = DisplayStyle.None;
    }

    public void Bind(ConstructionSite constructionSite, Vector3Int cell, Tilemap sourceTilemap, MapManager mapManager)
    {
        this.site = constructionSite;
        this.cellPosition = cell;
        this.tilemap = sourceTilemap;
        this.mapManager = mapManager;
    }

    private void Update()
    {
        if (site == null)
            return;

        UpdateScreenPosition();

        bool shouldShow = site.Progress > 0f;

        if (shouldShow != _isVisible)
        {
            _isVisible = shouldShow;
            root.style.display = _isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        if (_isVisible)
            Refresh();

        if (site.IsCompleted)
            FinishConstruction();

    }

    private void UpdateScreenPosition()
    {
        Vector3 worldPos = tilemap.GetCellCenterWorld(cellPosition);
        worldPos += new Vector3(0f, heightOffset, 0f);

        Vector3 screenPos = targetCamera.WorldToScreenPoint(worldPos);

        root.style.left = screenPos.x - 30;
        root.style.top = Screen.height - screenPos.y - 20;
    }

    private void Refresh()
    {
        float progress = site.Progress;

        progressfill.style.width = Length.Percent(progress);
        label.text = $"{Mathf.RoundToInt(progress)}%";

        AnimateProgressBar();
    }

    private void AnimateProgressBar()
    {
        if (site == null || progressShine == null)
            return;

        if (!site.IsBeingWorked)
        {
            progressShine.style.display = DisplayStyle.None;
            return;
        }

        progressShine.style.display = DisplayStyle.Flex;

        float barWidth = progressBackground.worldBound.width;

        if (barWidth <= 1f)
            return;

        float shineWidth = 4f;
        float speed = 120;

        float x = (Time.time * speed) % (barWidth + shineWidth) - shineWidth;

        progressShine.style.translate = new Translate(x, 0, 0);
    }


    private void FinishConstruction()
    {
        mapManager.RemoveStructure(site, site.Position);
        mapManager.AddStructure(site.TargetStructure, site.Position);
        Destroy(this.gameObject);
    }
}