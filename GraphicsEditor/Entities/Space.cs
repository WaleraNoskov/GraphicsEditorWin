using OpenCvSharp;

namespace GraphicsEditor.Entities;

public class Space : IDisposable
{
    public Mat Original { get; set; }

    public IDictionary<Filter, float> Filters { get; set; }

    public Mat Filtered { get; set; }
    
    public Space(string path)
    {
        var original = new Mat(path);
        Original = new Mat();
        Cv2.CvtColor(original, Original, ColorConversionCodes.BGR2BGRA);

        var filtered = new Mat(path);
        Filtered = new Mat();
        Cv2.CvtColor(filtered, Filtered, ColorConversionCodes.BGR2BGRA);
        
        Filters = new Dictionary<Filter, float>();
    }
    
    public Space()
    {
        Original = new Mat();
        Filtered = new Mat();
        Filters = new Dictionary<Filter, float>();
    }

    #region Dispose

    public void Dispose()
    {
        Original.Dispose();
        Filtered.Dispose();
    }
    
    ~Space() => Dispose();

    #endregion
}