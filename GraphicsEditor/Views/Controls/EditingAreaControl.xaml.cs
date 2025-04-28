using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphicsEditor.Entities;
using GraphicsEditor.ViewModels;
using OpenCvSharp.WpfExtensions;
using Frame = GraphicsEditor.Entities.Frame;

namespace GraphicsEditor.Views.Controls;

public partial class EditingAreaControl : UserControl
{
    private MainViewModel ViewModel => (DataContext as MainViewModel)!;
    
    private bool _mouseDown;
    private Point _mouseDownPos;

    public EditingAreaControl()
    {
        InitializeComponent();
    }

    public void RefreshCanvases()
    {
        LayersGrid.Children.Clear();

        if (!ViewModel.Layers.Any())
            return;

        foreach (var layer in ViewModel.Layers)
        {
            var source = layer.Filtered.ToWriteableBitmap();
            var image = new Image { Source = source, Width = source.PixelWidth, Height = source.PixelHeight };
            
            LayersGrid.Children.Add(image);
            
            Canvas.SetLeft(image, layer.Left);
            Canvas.SetTop(image, layer.Top);
        }
    }

    private void EditingAreaControl_OnInitialized(object? sender, EventArgs e)
    {
        Dispatcher.InvokeAsync(RefreshCanvases);
    }

    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Capture and track the mouse.
        _mouseDown = true;
        _mouseDownPos = e.GetPosition(TheGrid);
        TheGrid.CaptureMouse();

        // Initial placement of the drag selection box.         
        Canvas.SetLeft(SelectionBox, _mouseDownPos.X);
        Canvas.SetTop(SelectionBox, _mouseDownPos.Y);
        SelectionBox.Width = 0;
        SelectionBox.Height = 0;

        // Make the drag selection box visible.
        SelectionBox.Visibility = Visibility.Visible;
    }

    private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if(!_mouseDown)
            return;
        
        // Release the mouse capture and stop tracking it.
        _mouseDown = false;
        TheGrid.ReleaseMouseCapture();

        // Hide the drag selection box.
        SelectionBox.Visibility = Visibility.Collapsed;

        var mouseUpPos = e.GetPosition(TheGrid);

        var selectionArea = new Frame
        {
            X1 = Convert.ToInt32(_mouseDownPos.X),
            Y1 = Convert.ToInt32(_mouseDownPos.Y),
            X2 = Convert.ToInt32(mouseUpPos.X),
            Y2 = Convert.ToInt32(mouseUpPos.Y)
        };

        ViewModel.DivideLayerCommand.Execute(selectionArea);
    }

    private void Grid_MouseMove(object sender, MouseEventArgs e)
    {
        if (_mouseDown)
        {
            // When the mouse is held down, reposition the drag selection box.

            Point mousePos = e.GetPosition(TheGrid);

            if (_mouseDownPos.X < mousePos.X)
            {
                Canvas.SetLeft(SelectionBox, _mouseDownPos.X);
                SelectionBox.Width = mousePos.X - _mouseDownPos.X;
            }
            else
            {
                Canvas.SetLeft(SelectionBox, mousePos.X);
                SelectionBox.Width = _mouseDownPos.X - mousePos.X;
            }

            if (_mouseDownPos.Y < mousePos.Y)
            {
                Canvas.SetTop(SelectionBox, _mouseDownPos.Y);
                SelectionBox.Height = mousePos.Y - _mouseDownPos.Y;
            }
            else
            {
                Canvas.SetTop(SelectionBox, mousePos.Y);
                SelectionBox.Height = _mouseDownPos.Y - mousePos.Y;
            }
        }
    }
}