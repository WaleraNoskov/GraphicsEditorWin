using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using GraphicsEditor.Infrastructure;
using GraphicsEditor.ViewModels;
using OpenCvSharp.WpfExtensions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace GraphicsEditor.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private MainViewModel ViewModel => (DataContext as MainViewModel)!;

    public MainWindow(MainViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        Loaded += (_, _) =>
        {
            SystemThemeWatcher.Watch(this);
            ApplicationThemeManager.Apply(ApplicationTheme.Dark, WindowBackdropType.Tabbed);
        };
    }

    private void MainWindow_OnInitialized(object? sender, EventArgs e)
    {
        ViewModel.PropertyChanged += PropertyChangedHandler;
    }

    private void PropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
    {
        if ((e.PropertyName is nameof(ViewModel.Filters) or nameof(ViewModel.Layers)) && ViewModel.ImageIsOpened)
            Dispatcher.InvokeAsync(RefreshCanvases);
    }

    private void RefreshCanvases()
    {
        LayersGrid.Children.Clear();

        if (!ViewModel.Layers.Any())
            return;

        for (var i = ViewModel.Layers.Count - 1; i >= 0; i--)
            LayersGrid.Children.Add(new Image { Source = ViewModel.Layers[i].Filtered.ToWriteableBitmap() });
    }

    private void GrayscaleDefaultButton_OnClick(object sender, RoutedEventArgs e) => GrayscaleSlider.Value = DefaultFilterValues.DefaultGrayscalePercent;

    private void BrightnessDefaultButton_OnClick(object sender, RoutedEventArgs e) => BrightnessSlider.Value = DefaultFilterValues.DefaultBrightnessPercent;

    private void ContrastDefaultButton_OnClick(object sender, RoutedEventArgs e) => ContrastSlider.Value = DefaultFilterValues.DefaultContrastPercent;

    private void DraggingTitle_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}