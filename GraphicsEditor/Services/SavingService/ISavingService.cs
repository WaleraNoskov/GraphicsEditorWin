using GraphicsEditor.Entities;
using OpenCvSharp;

namespace GraphicsEditor.Services;

public interface ISavingService
{
    bool SaveAsJpg(Mat mat, string filename, int quality);

    bool SaveAsPng(Mat mat, string filename, int compressionLevel);

    bool SaveAsTiff(Mat mat, string filename);

    Mat ClueObjects(ICollection<GraphicObject> graphicObjects);
}