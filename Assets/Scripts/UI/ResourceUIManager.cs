using UnityEngine;
using UnityEngine.UIElements;
using UITKUtils;

class ResourceUIManager : MonoBehaviour
{
    public enum TypeResource
    {
        Food,
        Wood,
        Stone,
        Copper,
    }

    [SerializeField] private string _foodLabelName = "food_label";
    [SerializeField] private string _woodLabelName = "wood_label";
    [SerializeField] private string _stoneLabelName = "stone_label";
    [SerializeField] private string _copperLabelName = "copper_label";

    private Label _foodLabel, _woodLabel, _stoneLabel, _copperLabel;

    ResourceManager _resources;

    void Start()
    {
        // Root.
        VisualElement _root = GetComponent<UIDocument>().rootVisualElement;
        if (_root == null) { Debug.LogError("Missing references."); return; }

        _foodLabel = _root.Q<Label>(_foodLabelName);
        Validation.CheckQuery(_foodLabel, _foodLabelName);
        _woodLabel = _root.Q<Label>(_woodLabelName);
        Validation.CheckQuery(_woodLabel, _woodLabelName);
        _stoneLabel = _root.Q<Label>(_stoneLabelName);
        Validation.CheckQuery(_stoneLabel, _stoneLabelName);
        _copperLabel = _root.Q<Label>(_copperLabelName);
        Validation.CheckQuery(_copperLabel, _copperLabelName);

        //Manager
        _resources = FindAnyObjectByType<ResourceManager>();
    }

    private void Update()
    {
        if (_resources != null) {
            SetText(TypeResource.Food, _resources.Food.ToString());
            SetText(TypeResource.Wood, _resources.Wood.ToString());
            SetText(TypeResource.Stone, _resources.Stone.ToString());
            SetText(TypeResource.Copper, _resources.Copper.ToString());
        }
    }

    public void SetText(TypeResource type, string text)
    {
        Label label = null;
        
        switch (type)
        {   
            case TypeResource.Food:
                label = _foodLabel;
                break;
            case TypeResource.Wood:
                label = _woodLabel;
                break;
            case TypeResource.Stone:
                label = _stoneLabel;
                break;
            case TypeResource.Copper:
                label = _copperLabel;
                break;

        }

        label.text = text;
    }
}

