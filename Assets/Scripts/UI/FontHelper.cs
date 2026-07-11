using UnityEngine;

public static class FontHelper
{
    private const string BuiltinFontPath = "LegacyRuntime.ttf";

    public static Font GetDefaultFont()
    {
        Font defaultFont = Resources.GetBuiltinResource<Font>(BuiltinFontPath);
        if (defaultFont == null)
        {
            Debug.LogWarning($"FontHelper: Built-in font '{BuiltinFontPath}' not found. Falling back to default Font.");
            defaultFont = Font.CreateDynamicFontFromOSFont("Arial", 16);
        }

        return defaultFont;
    }
}
