using GraphicsEditor.Entities;
using OpenCvSharp;

namespace GraphicsEditor.Infrastructure;

public static class Extensions
{
    public static int ToPercentage(this float value) => Convert.ToInt32(value * 100);

    public static float FromPercentage(this int value) => value / 100f;

    public static Rect ToRect(this Frame area) => new(Math.Min(area.X1, area.X2), Math.Min(area.Y1, area.Y2), Math.Abs(area.X1 - area.X2), Math.Abs(area.Y1 - area.Y2));
}