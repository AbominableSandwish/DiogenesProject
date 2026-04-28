using UnityEngine;
using UnityEngine.UIElements;

public class ConstructionSiteView : MonoBehaviour
{
    [SerializeField] private UIDocument document;

    private VisualElement root;
    private VisualElement fill;
    private Label label;

    private ConstructionSite site;

    private void Awake()
    {
        root = document.rootVisualElement;
        fill = root.Q<VisualElement>("construction-progress-fill");
        label = root.Q<Label>("construction-progress-label");
    }

    public void Bind(ConstructionSite constructionSite)
    {
        site = constructionSite;
        Refresh();
    }

    private void Update()
    {
        if (site == null)
            return;

        Refresh();

        if (site.IsCompleted)
            Destroy(gameObject);
    }

    private void Refresh()
    {
        float progress = site.Progress;

        fill.style.width = Length.Percent(progress);
        label.text = $"{Mathf.RoundToInt(progress)}%";
    }
}