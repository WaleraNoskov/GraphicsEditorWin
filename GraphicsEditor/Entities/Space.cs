using OpenCvSharp;

namespace GraphicsEditor.Entities;

public class Space : IDisposable
{
    public Mat Original { get; set; }

    public IDictionary<Filter, float> Filters { get; set; }

    public Mat Filtered { get; set; }
    
    public Space(string path)
    {
        Original = Cv2.ImRead(path, ImreadModes.Unchanged);
        Original.ConvertTo(Original, MatType.CV_8UC4);
        Cv2.CvtColor(Original, Original, ColorConversionCodes.BGR2BGRA);

        Filtered = Original.Clone();
        
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