namespace GraphicsEditor.Entities;

public class ProjectInfo
{
    public string Name { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public List<GraphicObject> Layers { get; set; }

    public ProjectInfo(string name, int width = 800, int height = 600)
    {
        Name = name;
        Width = width;
        Height = height;
        Layers = [new GraphicObject("Background", width, height)];
    }
}