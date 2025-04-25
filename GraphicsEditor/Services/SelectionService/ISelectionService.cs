using GraphicsEditor.Entities;
using OpenCvSharp;

namespace GraphicsEditor.Services;

public interface ISelectionService
{
    Mat GetSquare(Mat mat, SelectionArea selectionArea);

    void CutSquare(Mat mat, SelectionArea selectionArea);
}