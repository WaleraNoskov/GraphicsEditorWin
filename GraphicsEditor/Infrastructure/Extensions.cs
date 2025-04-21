namespace GraphicsEditor.Infrastructure;

public static class Extensions
{
    public static int ToPercentage(this float value) => Convert.ToInt32(value * 100);
    
    public static float FromPercentage(this int value) => value / 100f;
}