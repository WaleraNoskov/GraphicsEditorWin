namespace GraphicsEditor.Entities;

public class SelectionArea(int top, int left, int width, int height)
{
    public int Top { get; set; } = top;
    public int Left { get; set; } = left;
    public int Width { get; set; } = width;
    public int Height { get; set; } = height;
}