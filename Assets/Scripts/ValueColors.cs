using UnityEngine;

public static class ValueColors
{
    // 2048-like palette; adjust to taste
    public static Color GetColor(int value)
    {
        switch (value)
        {
            case 2: return Hex("#EEE4DA");
            case 4: return Hex("#EDE0C8");
            case 8: return Hex("#F2B179");
            case 16: return Hex("#F59563");
            case 32: return Hex("#F67C5F");
            case 64: return Hex("#F65E3B");
            case 128: return Hex("#EDCF72");
            case 256: return Hex("#EDCC61");
            case 512: return Hex("#EDC850");
            case 1024: return Hex("#EDC53F");
            case 2048: return Hex("#EDC22E");
            default:
                // For values > 2048, lerp to some bright colors
                float t = Mathf.Clamp01(Mathf.Log(value / 2048f + 1f, 2f));
                return Color.Lerp(Hex("#3C3A32"), Hex("#6BFFB8"), t);
        }
    }

    private static Color Hex(string hex)
    {
        Color c;
        if (ColorUtility.TryParseHtmlString(hex, out c)) return c;
        return Color.white;
    }
}