using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float realSecondsPerGameDay = 300f; // 5 min = 1 jour
    [SerializeField, Range(0f, 1f)] private float startTimeOfDay = 0.25f; // 0.25 = matin

    [Header("Day / Night")]
    [SerializeField, Range(0f, 24f)] private float sunriseHour = 6f;
    [SerializeField, Range(0f, 24f)] private float sunsetHour = 20f;

    public float SunriseHour => sunriseHour;
    public float SunsetHour => sunsetHour;

    public int Day { get; private set; } = 1;
    public float TimeOfDay { get; private set; } // 0 → 1

    public float Hour => TimeOfDay * 24f;

    public bool IsDaytime
    {
        get
        {
            float currentHour = Hour;

            return currentHour >= sunriseHour &&
                   currentHour < sunsetHour;
        }
    }

    public float DaylightFactor
    {
        get
        {
            float currentHour = Hour;

            // Nuit
            if (currentHour < sunriseHour || currentHour > sunsetHour)
                return 0f;

            // Progression journée
            return Mathf.InverseLerp(
                sunriseHour,
                sunsetHour,
                currentHour
            );
        }
    }

    public bool IsNighttime => !IsDaytime;

    private void Start()
    {
        TimeOfDay = startTimeOfDay;
    }

    public string GetFormattedTime()
    {
        int hour = Mathf.FloorToInt(Hour);
        int minute = Mathf.FloorToInt((Hour - hour) * 60f);

        return $"{hour:00}:{minute:00}";
    }

    private void Update()
    {
        float dayProgress = Time.deltaTime / realSecondsPerGameDay;

        TimeOfDay += dayProgress;

        if (TimeOfDay >= 1f)
        {
            TimeOfDay -= 1f;
            Day++;
        }
    }
}