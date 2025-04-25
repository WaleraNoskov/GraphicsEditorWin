using System.Windows.Controls;
using GraphicsEditor.ViewModels;
using OpenCvSharp.WpfExtensions;

namespace GraphicsEditor.Views.Controls;

public partial class EditingAreaControl : UserControl
{
    private MainViewModel ViewModel => (DataContext as MainViewModel)!;
    
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
            LayersGrid.Children.Add(new Image { Source = layer.Filtered.ToWriteableBitmap() });
        
        // for (var i = ViewModel.Layers.Count - 1; i >= 0; i--)
        //     LayersGrid.Children.Add(new Image { Source = ViewModel.Layers[i].Filtered.ToWriteableBitmap() });
    }

    private void EditingAreaControl_OnInitialized(object? sender, EventArgs e)
    {
        Dispatcher.InvokeAsync(RefreshCanvases);
    }
}