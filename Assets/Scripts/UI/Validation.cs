using UnityEngine;
using UnityEngine.UIElements;

namespace UITKUtils
{
    public class Validation
    {
        public static void CheckQuery(VisualElement element, string name)
        {
            if (element == null)
                Debug.LogWarning($"Missing element named {name}");
        }
        public static bool Ensure(params VisualElement[] toCheck)
        {
            foreach (var item in toCheck)
            {
                if (item == null)
                {
                    Debug.LogError("Missing references.");
                    return false;
                }
            }
            return true;
        }
    }
}