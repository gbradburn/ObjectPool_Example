using UnityEngine;

public static class ExtensionMethods 
{
    public static string BaseName(this GameObject go)
    {
        if (go == null) return string.Empty;
        return go.name.Replace("(Clone)", string.Empty) ?? string.Empty;
    }
}
