using System.Collections;

public interface IGenerationStep
{
    public string Name { get; }
    public float Weight { get; }

    IEnumerator Generate(BasicMap map, int width, int height, int seed, IGenerationReporter reporter, int yieldEvery = 1);
}

public interface IGenerationReporter
{
    void SetStep(string stepName);
    void ReportItem(string itemLabel);
    void ReportProgress(float stepProgress01); // 0..1 pour la step en cours
    void Log(string line);
}