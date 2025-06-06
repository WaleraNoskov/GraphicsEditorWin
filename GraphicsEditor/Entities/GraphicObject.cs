﻿using OpenCvSharp;

namespace GraphicsEditor.Entities;

public class GraphicObject : IDisposable, ICloneable
{
    public string Name { get; set; }
    public Mat Original { get; set; }
    public IDictionary<Filter, float> Filters { get; set; }
    public Mat Filtered { get; set; }
    public int Left { get; set; }
    public int Top { get; set; }
    public int Width => Original.Width;
    public int Height => Original.Height;

    public GraphicObject(string name, string path)
    {
        Name = name;
        Original = Cv2.ImRead(path, ImreadModes.Unchanged);
        Original.ConvertTo(Original, MatType.CV_8UC4);
        Cv2.CvtColor(Original, Original, ColorConversionCodes.BGR2BGRA);

        Filtered = Original.Clone();

        Filters = new Dictionary<Filter, float>();
    }

    public GraphicObject(string name, int width, int height, int left = 0, int top = 0)
    {
        Name = name;

        Left = left;
        Top = top;
        
        Original = new Mat(new Size(width, height), MatType.CV_8UC4, new Scalar(255, 255, 255, 255));
        Filtered = Original.Clone();

        Filters = new Dictionary<Filter, float>();
    }

    public GraphicObject()
    {
        Name = "Background";
        Original = new Mat();
        Filtered = new Mat();
        Filters = new Dictionary<Filter, float>();
    }

    public object Clone()
    {
        return new GraphicObject
        {
            Name = Name + "(copy)",
            Original = Original.Clone(),
            Filtered = Filtered.Clone(),
            Filters = new Dictionary<Filter, float>(Filters)
        };
    }

    #region Dispose

    public void Dispose()
    {
        Original.Dispose();
        Filtered.Dispose();
    }

    ~GraphicObject() => Dispose();

    #endregion
}