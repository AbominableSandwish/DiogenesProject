using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ConstructionSiteView : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float heightOffset = 0.8f;

    private VisualElement root;
    private VisualElement fill;
    private Label label;

    MapManager mapManager;
    private ConstructionSite site;
    private Vector3Int cellPosition;
    private Tilemap tilemap;
    private bool _isVisible = false;

    private void Awake()
    {
        root = document.rootVisualElement;
        fill = root.Q<VisualElement>("construction-progress-fill");
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

        fill.style.width = Length.Percent(progress);
        label.text = $"{Mathf.RoundToInt(progress)}%";
    }


    private void FinishConstruction()
    {
        mapManager.RemoveStructure(site, site.Position);
        mapManager.AddStructure(site.TargetStructure, site.Position);
        Destroy(this.gameObject);
    }
}