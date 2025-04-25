using GraphicsEditor.Entities;
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

    public Mat ClueObjects(ICollection<GraphicObject> graphicObjects)
    {
        var biggestWidth = graphicObjects.Max(g => g.Width);
        var biggestHeight = graphicObjects.Max(g => g.Height);
        var size = new Size(biggestWidth, biggestHeight);
        var image = new Mat(size, MatType.CV_8UC4);

        foreach (var graphicObject in graphicObjects)
            AddMatAbove(image, graphicObject.Filtered, graphicObject.Left, graphicObject.Top);
        
        return image;
    }
    
    private static void AddMatAbove(Mat background, Mat foreground, int x, int y)
    { 
        var width = foreground.Width;
        var height = foreground.Height;

        for (var j = 0; j < height; j++)
        {
            for (var i = 0; i < width; i++)
            {
                var fgPixel = foreground.Get<Vec4b>(j, i);
                var bgPixel = background.Get<Vec4b>(y + j, x + i);

                var alpha = fgPixel.Item3 / 255f;
                var invAlpha = 1f - alpha;

                var b = (byte)(fgPixel.Item0 * alpha + bgPixel.Item0 * invAlpha);
                var g = (byte)(fgPixel.Item1 * alpha + bgPixel.Item1 * invAlpha);
                var r = (byte)(fgPixel.Item2 * alpha + bgPixel.Item2 * invAlpha);
                var a = Math.Max(fgPixel.Item3, bgPixel.Item3); // Можно брать максимум или смешивать

                background.Set(y + j, x + i, new Vec4b(b, g, r, a));
            }
        }
    }
}