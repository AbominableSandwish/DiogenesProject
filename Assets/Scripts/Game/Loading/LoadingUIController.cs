using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingUIController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private int maxLines = 12;

    private Label _title;
    private Label _stepLabel;
    private ProgressBar _progress;
    private Label _logsLabel;

    private readonly Queue<string> _lines = new();
    private readonly StringBuilder _sb = new();

    private void Awake()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();

        var root = uiDocument.rootVisualElement;
        _title = root.Q<Label>("title");
        _stepLabel = root.Q<Label>("stepLabel");
        _progress = root.Q<ProgressBar>("progressBar");
        _logsLabel = root.Q<Label>("logsLabel");
    }

    public void SetTitle(string txt)
    {
        if (_title != null) _title.text = txt;
    }

    public void SetStep(string step)
    {
        if (_stepLabel != null) _stepLabel.text = step;
        AddLine($"▶ {step}");
    }

    /// <summary>p01: 0..1</summary>
    public void SetProgress(float p01)
    {
        p01 = Mathf.Clamp01(p01);
        if (_progress != null)
        {
            _progress.value = p01 * 100f;
            _progress.title = $"{Mathf.RoundToInt(p01 * 100f)}%";
        }
    }

    public void AddLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line) || _logsLabel == null) return;

        _lines.Enqueue(line);
        while (_lines.Count > maxLines) _lines.Dequeue();

        _sb.Clear();
        foreach (var l in _lines) _sb.AppendLine(l);

        _logsLabel.text = _sb.ToString();
    }
}