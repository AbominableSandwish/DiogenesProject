using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static Structure;

public class MapGenerationRunner : MonoBehaviour
{
    [Header("Generation")]
    [SerializeField] private GenerationMap generator;

    [Header("Map (source de width/height + dictionary)")]
    [SerializeField] private BasicMap mapProvider;
    // -> petit script chez toi qui expose ta StructureMap actuelle (Width/Height + Structures)

    [Header("UI Toolkit")]
    [SerializeField] private UIDocument uiDocument;

    private ProgressBar _progress;
    private Label _label;
    private Button _button;

    private Coroutine _running;

    private void Awake()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();

        var root = uiDocument.rootVisualElement;

        _progress = root.Q<ProgressBar>("genProgress");
        _label = root.Q<Label>("genLabel");
        _button = root.Q<Button>("genButton");

        if (_button != null)
            _button.clicked += OnGenerateClicked;

        SetUI(0f, "Prêt");
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.clicked -= OnGenerateClicked;
    }

    private void OnGenerateClicked()
    {
        if (generator == null || mapProvider == null)
        {
            Debug.LogError("Generator ou Map manquant.");
            return;
        }

        if (_running != null)
            StopCoroutine(_running);

        _running = StartCoroutine(GenerateRoutine());
    }

    private IEnumerator GenerateRoutine()
    {
        _button?.SetEnabled(false);

        var map = mapProvider;

        SetUI(0f, "Génération...");

        yield return generator.GenerateRoutine(
            map,
            map.Width,
            map.Height,
            p => SetUI(p, $"Génération: {Mathf.RoundToInt(p * 100f)}%"),
            yieldEvery: 1       // yield chaque colonne (ou augmente pour plus rapide)
        );

        SetUI(1f, "Terminé");
        _button?.SetEnabled(true);
        _running = null;
    }

    private void SetUI(float progress01, string text)
    {
        if (_progress != null)
            _progress.value = Mathf.Clamp01(progress01) * 100f; // ProgressBar = 0..100

        if (_label != null)
            _label.text = text;
    }
}