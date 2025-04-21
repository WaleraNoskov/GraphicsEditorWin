using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using GraphicsEditor.Infrastructure;
using GraphicsEditor.ViewModels;
using OpenCvSharp.WpfExtensions;

namespace GraphicsEditor.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel ViewModel => (DataContext as MainViewModel)!;

    public MainWindow(MainViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();

        RefreshCanvases();
    }

    private void MainWindow_OnInitialized(object? sender, EventArgs e)
    {
        ViewModel.PropertyChanged += PropertyChangedHandler;
    }

    private void PropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ViewModel.OriginImage) or nameof(ViewModel.EditedImage) or nameof(ViewModel.Filters))
            Dispatcher.InvokeAsync(RefreshCanvases);
    }

    private void RefreshCanvases()
    {
        Origin.Source = ViewModel.OriginImage.ToWriteableBitmap();
        Edited.Source = ViewModel.EditedImage.ToWriteableBitmap();
    }

    private void GrayscaleDefaultButton_OnClick(object sender, RoutedEventArgs e) => GrayscaleSlider.Value = DefaultFilterValues.DefaultGrayscalePercent;

    private void BrightnessDefaultButton_OnClick(object sender, RoutedEventArgs e) => BrightnessSlider.Value = DefaultFilterValues.DefaultBrightnessPercent;

    private void ContrastDefaultButton_OnClick(object sender, RoutedEventArgs e) => ContrastSlider.Value = DefaultFilterValues.DefaultContrastPercent;

    private void HidePreviewButton_OnPreviewMouseDown(object sender, MouseButtonEventArgs e) => Edited.Visibility = Visibility.Hidden;

    private void HidePreviewButton_OnPreviewMouseUp(object sender, MouseButtonEventArgs e) => Edited.Visibility = Visibility.Visible;

    private void ImagesGrid_OnPreviewMouseDown(object sender, MouseButtonEventArgs e) => Edited.Visibility = Visibility.Hidden;

    private void ImagesGrid_OnPreviewMouseUp(object sender, MouseButtonEventArgs e) => Edited.Visibility = Visibility.Visible;
}