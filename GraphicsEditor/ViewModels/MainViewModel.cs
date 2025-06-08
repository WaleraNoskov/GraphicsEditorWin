using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using GraphicsEditor.Entities;
using GraphicsEditor.Infrastructure;
using GraphicsEditor.Models;
using Microsoft.Win32;

namespace GraphicsEditor.ViewModels;

public class MainViewModel : PropertyObject
{
    private readonly MainModel _model;

    public MainViewModel(MainModel model)
    {
        _model = model;
        _model.PropertyChanged += (sender, args) =>
        {
            OnPropertyChanged(args.PropertyName ?? string.Empty);

            if (args.PropertyName == nameof(_model.Filters))
            {
                OnPropertyChanged(nameof(Grayscale));
                OnPropertyChanged(nameof(GrayscaleIsEnabled));
                OnPropertyChanged(nameof(Brightness));
                OnPropertyChanged(nameof(BrightnessIsEnabled));
                OnPropertyChanged(nameof(Contrast));
                OnPropertyChanged(nameof(ContrastIsEnabled));
            }
        };

        ResetCommand = new RelayCommand(OnResetCommandExecuted, CanResetCommandExecute);
        OpenImageDialogCommand = new RelayCommand(OnOpenImageDialogCommandExecuted, CanOpenImageDialogCommandExecute);
        SaveFileDialogCommand = new AsyncRelayCommand(OnSaveFileDialogCommandExecuted, CanSaveFileDialogCommandExecute);
    }
    
    public GraphicObject GraphicObject => _model.GraphicObject;

    public IReadOnlyDictionary<Filter, float> Filters
    {
        get
        {
            OnPropertyChanged(nameof(Grayscale));
            OnPropertyChanged(nameof(Contrast));
            OnPropertyChanged(nameof(Brightness));
            return _model.Filters;
        }
    }

    public bool GrayscaleIsEnabled => Grayscale != DefaultFilterValues.DefaultGrayscalePercent;

    public int Grayscale
    {
        get => _model.Filters is not null && _model.Filters.ContainsKey(Filter.Grayscale)
            ? _model.Filters[Filter.Grayscale].ToPercentage()
            : DefaultFilterValues.DefaultGrayscale.ToPercentage();
        set
        {
            _model.SetFilterAsync(Filter.Grayscale, value.FromPercentage());
            OnPropertyChanged(nameof(GrayscaleIsEnabled));
            OnPropertyChanged(nameof(ImageIsChanged));
        }
    }

    public bool BrightnessIsEnabled => Brightness != DefaultFilterValues.DefaultBrightnessPercent;

    public int Brightness
    {
        get => _model.Filters is not null && _model.Filters.ContainsKey(Filter.Brightness)
            ? Convert.ToInt32(_model.Filters[Filter.Brightness])
            : Convert.ToInt32(DefaultFilterValues.DefaultBrightness);
        set
        {
            _model.SetFilterAsync(Filter.Brightness, value);
            OnPropertyChanged(nameof(BrightnessIsEnabled));
            OnPropertyChanged(nameof(ImageIsChanged));
        }
    }

    public bool ContrastIsEnabled => Contrast != DefaultFilterValues.DefaultContrastPercent;

    public int Contrast
    {
        get => _model.Filters is not null && _model.Filters.ContainsKey(Filter.Contrast)
            ? _model.Filters[Filter.Contrast].ToPercentage()
            : DefaultFilterValues.DefaultContrast.ToPercentage();
        set
        {
            _model.SetFilterAsync(Filter.Contrast, value.FromPercentage());
            OnPropertyChanged(nameof(ContrastIsEnabled));
            OnPropertyChanged(nameof(ImageIsChanged));
        }
    }

    public bool ImageIsChanged => GraphicObject is not null && (GrayscaleIsEnabled || BrightnessIsEnabled || ContrastIsEnabled);

    #region ResetCommand

    public ICommand ResetCommand { get; set; }

    private void OnResetCommandExecuted()
    {
        _model.ResetFilters();
        _model.ReApplyFilters();
        OnPropertyChanged(nameof(ImageIsChanged));
    }

    private bool CanResetCommandExecute() => true;

    #endregion

    #region OpenImageDialog

    public ICommand OpenImageDialogCommand { get; set; }

    private void OnOpenImageDialogCommandExecuted()
    {
        var dialog = new OpenFileDialog
        {
            Multiselect = false,
            Filter = "Image files (*.png;*.jpeg,*.jpg,*.tiff)|*.png;*.jpeg;*.jpg;*.tiff;*tif"
        };

        var result = dialog.ShowDialog();
        if (result is not true)
            return;
        
        _model.OpenFile(dialog.FileName);
    }

    private bool CanOpenImageDialogCommandExecute() => true;

    #endregion

    #region SaveFileDialog

    public ICommand SaveFileDialogCommand { get; set; }

    private async Task OnSaveFileDialogCommandExecuted()
    {
        if (GraphicObject is null)
            return;
        
        var dialog = new SaveFileDialog();
        dialog.FileName = "Untitled";
        dialog.Filter = "Png files (*.png)|*.png|Jpeg files (*.jpeg)|*.jpeg|Tiff files (*.tiff)|*.tiff";

        var result = dialog.ShowDialog();
        if (result is not true)
            return;

        await _model.SaveImage(dialog.FileName);
    }

    private bool CanSaveFileDialogCommandExecute() => true;

    #endregion
}