using OpenCvSharp;

namespace GraphicsEditor.Entities;

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
            { Filter.Grayscale, 0 },
            { Filter.Alpha, 100 },
            { Filter.Brightness, 0f },
            { Filter.Contrast, 1f },
        };
    }
}