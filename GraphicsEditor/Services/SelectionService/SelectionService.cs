using GraphicsEditor.Entities;
using GraphicsEditor.Infrastructure;
using OpenCvSharp;

namespace GraphicsEditor.Services;

public class SelectionService : ISelectionService
{
    public Mat GetSquare(Mat mat, Frame selectionArea)
    {
        var selected = new Mat(mat, selectionArea.ToRect());
        return selected;
    }

    public void CutSquare(Mat mat, Frame selectionArea)
    {
        Cv2.Rectangle(mat, selectionArea.ToRect(), new Scalar(0, 0, 0, 0), -1);
    }
}