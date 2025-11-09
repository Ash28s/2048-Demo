using UnityEngine;

public enum BallColor
{
    Red,
    Black,
    White,
    Yellow,
    Blue,
    Green
}

public static class BallColorUtility
{
    // Define the actual colors for each ball type
    public static Color GetUnityColor(BallColor ballColor)
    {
        switch (ballColor)
        {
            case BallColor.Red:
                return new Color(1f, 0f, 0f); // Pure Red
            case BallColor.Black:
                return new Color(0.1f, 0.1f, 0.1f); // Dark Gray (pure black is invisible)
            case BallColor.White:
                return new Color(1f, 1f, 1f); // Pure White
            case BallColor.Yellow:
                return new Color(1f, 1f, 0.0f); // Yellow
            case BallColor.Blue:
                return new Color(0f, 0.5f, 1f); // Blue
            case BallColor.Green:
                return new Color(0f, 1f, 0f); // Green
            default:
                return Color.white;
        }
    }

    // Get a random ball color
    public static BallColor GetRandomColor()
    {
        int randomIndex = Random.Range(0, 6); // 0-5 for 6 colors
        return (BallColor)randomIndex;
    }
}