    using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenerationRunnerUITK : MonoBehaviour, IGenerationReporter
{
    [Header("Pipeline")]
    [SerializeField] private GenerationPipeline pipeline;

    [Header("Map")]
    [SerializeField] private BasicMap mapProvider;

    [Header("UI Toolkit")]
    [SerializeField] private UIDocument uiDocument;

    private ProgressBar _progress;
    private Label _label;
    private Button _button;
    private TextField _seedField;

    private int seed;

    private Coroutine _running;

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        var root = uiDocument.rootVisualElement;

        _progress = root.Q<ProgressBar>("genProgress");
        _label = root.Q<Label>("genLabel");
        _button = root.Q<Button>("genButton");
        _seedField = root.Q<TextField>("seedField");

        seed = UnityEngine.Random.Range(0, int.MaxValue);
        if (_seedField != null)
            _seedField.value = seed.ToString();

        if (_button != null)
            _button.clicked += OnGenerateClicked;

        UpdateUI(0f, "Prêt");
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.clicked -= OnGenerateClicked;
    }

    private void OnGenerateClicked()
    {
        if (_running != null)
            StopCoroutine(_running);

        if (_seedField == null)
            return;


        int seed = 0;
        if (int.TryParse(_seedField.value, out int value))
        {
            Debug.Log($"Seed = {value}");
            seed = value;
        }
        else
        {
            Debug.LogWarning("Invalid seed");
        }

        _running = StartCoroutine(GenerateRoutine(seed));
    }

    private IEnumerator GenerateRoutine(int seed = 0)
    {
        if (pipeline == null)
        {
            Debug.LogError("GenerationPipeline manquante.");
            yield break;
        }

        if (mapProvider == null)
        {
            Debug.LogError("BasicMap manquante.");
            yield break;
        }

        var map = mapProvider;

        Debug.Log($"Before init: width={map.Width}, height={map.Height}, structures null? {map.Structures == null}");

        if (map.Structures == null || map.Width <= 0 || map.Height <= 0)
        {
            var mapManager = MapManager.Instance;
            if (mapManager == null)
            {
                Debug.LogError("MapManager.Instance est null. Impossible d'initialiser BasicMap.");
                yield break;
            }

            map.Init(mapManager.Width, mapManager.Height);
        }

        Debug.Log($"After init: width={map.Width}, height={map.Height}, count={map.Structures.Count}");

        var steps = pipeline.Steps;

        if (steps == null || steps.Count == 0)
        {
            Debug.LogError("La pipeline ne contient aucune étape.");
            yield break;
        }

        _button?.SetEnabled(false);

        map.ClearMap();

        int totalSteps = steps.Count;

        for (int i = 0; i < totalSteps; i++)
        {
            GenerationStep step = steps[i];
            if (step == null)
                continue;

            int stepNumber = i + 1;

            int stepSeed = pipeline.Seed + i;

            SetStep($"[{stepNumber}/{totalSteps}] {step.Name}");
            ReportItem(string.Empty);
            ReportProgress(0f);

            yield return null;

            yield return StartCoroutine(step.DoGenerate(
                map,
                map.Width,
                map.Height,
                seed,
                this,
                yieldEvery: 1
            ));

            ReportProgress(1f);
            yield return null;
        }

        UpdateUI(1f, "Terminé");
        _button?.SetEnabled(true);
        _running = null;
    }

    private void UpdateUI(float progress01, string text)
    {
        if (_progress != null)
            _progress.value = Mathf.Clamp01(progress01) * 100f;

        if (_label != null)
            _label.text = text;
    }

    public void SetStep(string stepName)
    {
        if (_label != null)
            _label.text = stepName;
    }

    public void ReportItem(string itemLabel)
    {
        if (_label == null)
            return;

        if (string.IsNullOrEmpty(itemLabel))
            return;

        _label.text = $"{_label.text}\n{itemLabel}";
    }

    public void ReportProgress(float stepProgress01)
    {
        if (_progress != null)
            _progress.value = Mathf.Clamp01(stepProgress01) * 100f;
    }

    public void Log(string line)
    {
        Debug.Log(line);
    }
}