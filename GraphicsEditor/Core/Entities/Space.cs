using OpenCvSharp;

namespace GraphicsEditor.Core.Entities;

public class Space
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

        Filters = new Dictionary<Filter, float>
        {
            { Entities.Filter.Alpha, 100},
            { Entities.Filter.Brightness, 0f},
            { Entities.Filter.Contrast, 1f},
        };
    }
}