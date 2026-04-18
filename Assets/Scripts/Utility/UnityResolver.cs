using UnityEngine;

public static class UnityResolver
{
    public static T Resolve<T>(MonoBehaviour context, T reference, string label) where T : Object
    {
        if (reference != null)
            return reference;

        T found = Object.FindAnyObjectByType<T>();

        if (found == null)
        {
            Debug.LogError($"{label} reference is missing and no instance was found in the scene.", context);
        }

        return found;
    }
}