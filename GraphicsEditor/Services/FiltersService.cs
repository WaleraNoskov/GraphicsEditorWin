using OpenCvSharp;

namespace GraphicsEditor.Services;

public class FiltersService : IFiltersService
{
    public void ApplyGrayscale(Mat mat, float mix)
    {
        // if (mat.Type() != MatType.CV_8UC4)
        //     throw new ArgumentException("Input must be BGRA (CV_8UC4)");

        // Извлекаем каналы
        var alpha = Cv2.Split(mat)[3];

        // Считаем оттенок серого вручную (можно использовать любой метод)
        var gray = new Mat();
        Cv2.CvtColor(mat, gray, ColorConversionCodes.BGRA2GRAY); // 1 канал
        
        // Собираем BGRA, где B = G = R = gray, и оригинальный A
        var grayed = new Mat();
        Cv2.Merge([gray, gray, gray, alpha], grayed); // Теперь снова CV_8UC4
        
        Cv2.AddWeighted(grayed, mix, mat, 1.0 - mix, 0, mat);
    }
    
    public void ApplyAlpha(Mat mat, float alpha)
    {
        // if (mat.Type() != MatType.CV_8UC4)
        //     throw new ArgumentException("Input must be BGRA (CV_8UC4)");

        // Извлекаем каналы
        var channels = Cv2.Split(mat);
        var b = channels[0];
        var g = channels[1];
        var r = channels[2];
        var a = channels[3];

        // Считаем оттенок серого вручную (можно использовать любой метод)
        var gray = new Mat();
        Cv2.CvtColor(mat, gray, ColorConversionCodes.BGRA2GRAY); // 1 канал
        
        // Изменим прозрачность (умножим все значения альфа-канала на 0.5)
        a.ConvertTo(a, MatType.CV_8UC1, alpha); // Масштабируем значения
        
        // Собираем BGRA, где B = G = R = gray, и оригинальный A
        var grayed = new Mat();
        Cv2.Merge([b, g, r, a], grayed); // Теперь снова CV_8UC4
        
        grayed.CopyTo(mat);
    }

    public void ApplyBrightnessAndContrast(Mat mat, float contrast, float brightness)
    {
        Cv2.ConvertScaleAbs(mat, mat, contrast, brightness);
    }
}