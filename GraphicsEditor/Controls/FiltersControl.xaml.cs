using System.Windows;
using System.Windows.Controls;
using GraphicsEditor.Infrastructure;

namespace GraphicsEditor.Controls;

public partial class FiltersControl : UserControl
{
    public FiltersControl()
    {
        InitializeComponent();
    }
    
    private void GrayscaleDefaultButton_OnClick(object sender, RoutedEventArgs e) => GrayscaleSlider.Value = DefaultFilterValues.DefaultGrayscalePercent;

    private void BrightnessDefaultButton_OnClick(object sender, RoutedEventArgs e) => BrightnessSlider.Value = DefaultFilterValues.DefaultBrightnessPercent;

    private void ContrastDefaultButton_OnClick(object sender, RoutedEventArgs e) => ContrastSlider.Value = DefaultFilterValues.DefaultContrastPercent;
}