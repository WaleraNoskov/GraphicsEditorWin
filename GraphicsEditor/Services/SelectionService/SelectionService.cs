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
        var rect = selectionArea.ToRect();
        rect.Left += 1;
        rect.Width -= 2;
        rect.Top += 1;
        rect.Height -= 2;
        Cv2.Rectangle(mat, rect, new Scalar(0, 0, 0, 0), -1);
    }
}