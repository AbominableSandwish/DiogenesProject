using UnityEngine;

public class DayNightOverlay : MonoBehaviour
{
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private SpriteRenderer overlay;

    [Header("Colors")]
    [SerializeField] private Color dayColor = new(1f, 1f, 1f, 0f);
    [SerializeField] private Color nightColor = new(0.05f, 0.08f, 0.2f, 0.45f);

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 2f;

    private void Update()
    {
        float currentHour = timeManager.Hour;

        float sunrise = timeManager.SunriseHour;
        float sunset = timeManager.SunsetHour;

        float lightFactor;

        // =========================
        // Nuit
        // =========================
        if (currentHour < sunrise || currentHour >= sunset)
        {
            lightFactor = 0f;
        }
        // =========================
        // Lever du soleil
        // =========================
        else if (currentHour < sunrise + transitionDuration)
        {
            lightFactor = Mathf.InverseLerp(
                sunrise,
                sunrise + transitionDuration,
                currentHour
            );
        }
        // =========================
        // Coucher du soleil
        // =========================
        else if (currentHour > sunset - transitionDuration)
        {
            lightFactor = Mathf.InverseLerp(
                sunset,
                sunset - transitionDuration,
                currentHour
            );
        }
        // =========================
        // Jour
        // =========================
        else
        {
            lightFactor = 1f;
        }

        overlay.color = Color.Lerp(
            nightColor,
            dayColor,
            lightFactor
        );
    }
}