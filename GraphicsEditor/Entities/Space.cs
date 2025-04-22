using OpenCvSharp;

namespace GraphicsEditor.Entities;

public class Space : IDisposable
{
    public Mat Original { get; set; }

    public IDictionary<Filter, float> Filters { get; set; }

    public Mat Filtered { get; set; }
    
    public Space(string path)
    {
        Original = new Mat(path);
        Original.ConvertTo(Original, MatType.CV_8UC4);
        Cv2.CvtColor(Original, Original, ColorConversionCodes.BGR2BGRA);

        Filtered = new Mat(path);
        Filtered.ConvertTo(Filtered, MatType.CV_8UC4);
        Cv2.CvtColor(Filtered, Filtered, ColorConversionCodes.BGR2BGRA);
        
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