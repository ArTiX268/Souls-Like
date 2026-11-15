using UnityEngine;

namespace ArTiX.Utils
{
    public static class ErrorMessage
    {
        public static void MissingReferenceWarning(string variableName, string componentName, GameObject gameObject)
        {
            Debug.LogWarning($"The variable {variableName} of the component {componentName} on {gameObject.name} is null.");
        }
    }
}
