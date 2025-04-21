using OpenCvSharp;

namespace GraphicsEditor.Services;

public interface IFiltersService
{
    void ApplyGrayscale(Mat mat, float mix);

    void ApplyAlpha(Mat mat, float alpha);
    
    void ApplyBrightnessAndContrast(Mat mat, float contrast, float brightness);
}