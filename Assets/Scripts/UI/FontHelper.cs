using UnityEngine;
using UnityEngine.UI;

public static class FontHelper
{
    private const string BuiltinFontPath = "LegacyRuntime.ttf";
    private const string BuiltinUISpritePath = "UI/Skin/UISprite.psd";

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

    public static Sprite GetDefaultUISprite()
    {
        Sprite defaultSprite = Resources.GetBuiltinResource<Sprite>(BuiltinUISpritePath);
        if (defaultSprite == null)
        {
            Debug.LogWarning($"FontHelper: Built-in UI sprite '{BuiltinUISpritePath}' not found.");
        }

        return defaultSprite;
    }

    public static void ApplyDefaultUISprite(Image image)
    {
        if (image == null)
            return;

        Sprite sprite = GetDefaultUISprite();
        if (sprite != null)
        {
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
        }
    }
}
