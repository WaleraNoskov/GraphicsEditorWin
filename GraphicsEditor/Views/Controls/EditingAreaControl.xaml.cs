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
        _globalMouseDownPos = e.GetPosition(TheGrid);
        _localMouseDownPos = e.GetPosition(LayersGrid.Children[ViewModel.Layers.IndexOf(ViewModel.SelectedLayer)]);
        TheGrid.CaptureMouse();

        // Initial placement of the drag selection box.         
        Canvas.SetLeft(SelectionBox, _globalMouseDownPos.X);
        Canvas.SetTop(SelectionBox, _globalMouseDownPos.Y);
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
        var localMouseUpPos = e.GetPosition(LayersGrid.Children[ViewModel.Layers.IndexOf(ViewModel.SelectedLayer)]);

        var selectionArea = new SelectionArea(
            Convert.ToInt32 (Math.Min(_localMouseDownPos.Y, localMouseUpPos.Y)),
            Convert.ToInt32(Math.Min(_localMouseDownPos.X, localMouseUpPos.X)),
            Convert.ToInt32(Math.Abs(localMouseUpPos.X - _localMouseDownPos.X)),
            Convert.ToInt32(Math.Abs(localMouseUpPos.Y - _localMouseDownPos.Y))
        );
        
        ViewModel.DivideLayerCommand.Execute(selectionArea);
    }

    private void Grid_MouseMove(object sender, MouseEventArgs e)
    {
        if (_mouseDown)
        {
            // When the mouse is held down, reposition the drag selection box.

            Point mousePos = e.GetPosition(TheGrid);

            if (_globalMouseDownPos.X < mousePos.X)
            {
                Canvas.SetLeft(SelectionBox, _globalMouseDownPos.X);
                SelectionBox.Width = mousePos.X - _globalMouseDownPos.X;
            }
            else
            {
                Canvas.SetLeft(SelectionBox, mousePos.X);
                SelectionBox.Width = _globalMouseDownPos.X - mousePos.X;
            }

            if (_globalMouseDownPos.Y < mousePos.Y)
            {
                Canvas.SetTop(SelectionBox, _globalMouseDownPos.Y);
                SelectionBox.Height = mousePos.Y - _globalMouseDownPos.Y;
            }
            else
            {
                Canvas.SetTop(SelectionBox, mousePos.Y);
                SelectionBox.Height = _globalMouseDownPos.Y - mousePos.Y;
            }
        }
    }
}