using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphicsEditor.Entities;
using GraphicsEditor.ViewModels;
using OpenCvSharp.WpfExtensions;

namespace GraphicsEditor.Views.Controls;

public partial class EditingAreaControl : UserControl
{
    private MainViewModel ViewModel => (DataContext as MainViewModel)!;
    
    private bool _mouseDown;
    private Point _globalMouseDownPos;
    private Point _localMouseDownPos;

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
            LayersGrid.Children.Add(new Image { Source = source,  Width = source.PixelWidth, Height = source.PixelHeight });
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
        _globalMouseDownPos = e.GetPosition(theGrid);
        _localMouseDownPos = e.GetPosition(LayersGrid.Children[ViewModel.Layers.IndexOf(ViewModel.SelectedLayer)]);
        theGrid.CaptureMouse();

        // Initial placement of the drag selection box.         
        Canvas.SetLeft(selectionBox, _globalMouseDownPos.X);
        Canvas.SetTop(selectionBox, _globalMouseDownPos.Y);
        selectionBox.Width = 0;
        selectionBox.Height = 0;

        // Make the drag selection box visible.
        selectionBox.Visibility = Visibility.Visible;
    }

    private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if(!_mouseDown)
            return;
        
        // Release the mouse capture and stop tracking it.
        _mouseDown = false;
        theGrid.ReleaseMouseCapture();

        // Hide the drag selection box.
        selectionBox.Visibility = Visibility.Collapsed;

        var mouseUpPos = e.GetPosition(theGrid);
        var localMouseUpPos = e.GetPosition(LayersGrid.Children[ViewModel.Layers.IndexOf(ViewModel.SelectedLayer)]);

        var selectionArea = new SelectionArea(
            Convert.ToInt32 (Math.Min(_localMouseDownPos.Y, localMouseUpPos.Y)),
            Convert.ToInt32(Math.Min(_localMouseDownPos.X, localMouseUpPos.X)),
            Convert.ToInt32(Math.Abs(localMouseUpPos.X - _localMouseDownPos.X)),
            Convert.ToInt32(Math.Abs(localMouseUpPos.Y - _localMouseDownPos.Y))
        );
        
        ViewModel.CropCommand.Execute(selectionArea);
    }

    private void Grid_MouseMove(object sender, MouseEventArgs e)
    {
        if (_mouseDown)
        {
            // When the mouse is held down, reposition the drag selection box.

            Point mousePos = e.GetPosition(theGrid);

            if (_globalMouseDownPos.X < mousePos.X)
            {
                Canvas.SetLeft(selectionBox, _globalMouseDownPos.X);
                selectionBox.Width = mousePos.X - _globalMouseDownPos.X;
            }
            else
            {
                Canvas.SetLeft(selectionBox, mousePos.X);
                selectionBox.Width = _globalMouseDownPos.X - mousePos.X;
            }

            if (_globalMouseDownPos.Y < mousePos.Y)
            {
                Canvas.SetTop(selectionBox, _globalMouseDownPos.Y);
                selectionBox.Height = mousePos.Y - _globalMouseDownPos.Y;
            }
            else
            {
                Canvas.SetTop(selectionBox, mousePos.Y);
                selectionBox.Height = _globalMouseDownPos.Y - mousePos.Y;
            }
        }
    }
}