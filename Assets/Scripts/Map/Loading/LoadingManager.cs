/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour, ILoadReporter
{
    [SerializeField] private string nextSceneName = "Game";
    [SerializeField] private LoadingUIController ui;

    private readonly List<ILoadStep> _steps = new();

    private float _totalWeight;
    private float _doneWeight;
    private float _currentWeight;
    private float _currentStepProgress01;

    private void Awake()
    {
        UnityResolver.Resolve(ui, this, "LoadingUIController");

        // Enregistre tes steps ici
        _steps.Add(new LoadTilesAddressablesStep("Tile"));
        if (FindAnyObjectByType<SceneRegistry>())
        {
            _steps.Add(new LoadScenesAddressablesStep("SceneTest"));
        }
      
        //_steps.Add(new LoadTilesStep());          // Resources.LoadAll ou Addressables (selon ton choix)
        //_steps.Add(new LoadScriptablesStep());    // exemple

        _totalWeight = 0f;
        foreach (var s in _steps) _totalWeight += Mathf.Max(0.0001f, s.Weight);
    }

    private void Start()
    {
        ui?.SetTitle("Chargement...");
        StartCoroutine(RunLoading());
    }

    private IEnumerator RunLoading()
    {
        ui?.AddLine("Initialisation...");

        foreach (var step in _steps)
        {
            _currentWeight = Mathf.Max(0.0001f, step.Weight);
            _currentStepProgress01 = 0f;

            SetStep(step.Name);
            UpdateGlobalProgress();

            yield return null; // ✅ laisse l’UI Toolkit rafraîchir au moins 1 frame

            yield return step.Execute(this);

            _doneWeight += _currentWeight;
            _currentStepProgress01 = 1f;
            UpdateGlobalProgress();
        }

        ui?.AddLine("Ouverture de la scène...");
        yield return LoadSceneAsync(nextSceneName);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            // Optionnel: tu peux mixer la progression du load scene à la fin.
            // Ici on affiche juste un "100%" quand la scène arrive.
            yield return null;
        }
    }

    // --- ILoadReporter ---
    public void SetStep(string stepName)
    {
        ui?.SetStep(stepName);
    }

    public void ReportItem(string itemLabel)
    {
        ui?.AddLine(itemLabel);
    }

    public void ReportProgress(float stepProgress01)
    {
        _currentStepProgress01 = Mathf.Clamp01(stepProgress01);
        UpdateGlobalProgress();
    }

    public void Log(string line) => ui?.AddLine(line);

    private void UpdateGlobalProgress()
    {
        float global = (_doneWeight + (_currentStepProgress01 * _currentWeight)) / Mathf.Max(0.0001f, _totalWeight);
        ui?.SetProgress(global);
    }
}