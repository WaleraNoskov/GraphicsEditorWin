using GraphicsEditor.Entities;
using OpenCvSharp;

namespace GraphicsEditor.Services;

public interface ISelectionService
{
    Mat GetSquare(Mat mat, Frame selectionArea);

    void CutSquare(Mat mat, Frame selectionArea);
}