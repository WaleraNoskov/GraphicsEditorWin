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
        if ((e.PropertyName is nameof(ViewModel.Filters) or nameof(ViewModel.Layers)) && ViewModel.SelectedLayer is not null)
            Dispatcher.InvokeAsync(EditingAreaControl.RefreshCanvases);
    }

    

    private void DraggingTitle_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}