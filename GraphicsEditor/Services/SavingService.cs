using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace GraphicsEditor.Services;

public class SavingService : ISavingService
{
    public bool SaveAsJpg(Mat mat, string filename, int quality)
    {
        var matToSave = mat;

        // Если CV_8UC4 — убираем альфу
        if (mat.Type() == MatType.CV_8UC4)
        {
            var channels = Cv2.Split(mat);
            Cv2.Merge(new[] { channels[0], channels[1], channels[2] }, matToSave); // BGR
        }

        // Настройки качества (0 - худшее, 100 - лучшее)
        var parameters = new ImageEncodingParam(ImwriteFlags.JpegQuality, quality);

        // Сохраняем Mat в JPEG
        Cv2.ImWrite(filename, matToSave, new[] { parameters });

        // Если создавали копию — чистим
        if (matToSave != mat)
            matToSave.Dispose();
        
        return true;
    }

    public bool SaveAsPng(Mat mat, string filename, int compressionLevel)
    {
        var parameters = new ImageEncodingParam(ImwriteFlags.PngCompression, compressionLevel);
        Cv2.ImWrite(filename, mat, new[] { parameters });
        
        return true;
    }

    public bool SaveAsTiff(Mat mat, string filename)
    {
        Cv2.ImWrite(filename, mat);
        
        return true;
    }
}