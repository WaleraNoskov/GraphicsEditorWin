using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.Input;
using GraphicsEditor.Entities;
using GraphicsEditor.Infrastructure;
using GraphicsEditor.Models;
using Microsoft.Win32;
using OpenCvSharp;

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
        DeleteLayerCommand = new RelayCommand(OnDeleteLayerCommandExecuted, CanDeleteLayerCommandExecute);
        DuplicateLayerCommand = new RelayCommand(OnDuplicateLayerCommandExecuted, CanDuplicateLayerCommandExecute);
    }

    public ObservableCollection<GraphicObject> Layers => _model.Layers;

    public GraphicObject SelectedLayer
    {
        get => _model.SelectedLayer;
        set => _model.SelectLayer(value);
    }

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
        get => _model.Filters[Filter.Grayscale].ToPercentage();
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
        get => (int)_model.Filters[Filter.Brightness];
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
        get => _model.Filters[Filter.Contrast].ToPercentage();
        set
        {
            _model.SetFilterAsync(Filter.Contrast, value.FromPercentage());
            OnPropertyChanged(nameof(ContrastIsEnabled));
            OnPropertyChanged(nameof(ImageIsChanged));
        }
    }
    
    public bool ImageIsChanged => SelectedLayer is not null && (GrayscaleIsEnabled || BrightnessIsEnabled || ContrastIsEnabled);

    #region ResetCommand

    public ICommand ResetCommand { get; set; }

    private void OnResetCommandExecuted()
    {
        _model.ResetAndReapplyFilters();
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

        _model.AddLayerFromFile(dialog.FileName);
    }

    private bool CanOpenImageDialogCommandExecute() => true;

    #endregion

    #region DeleteLayer

    public ICommand DeleteLayerCommand { get; set; }

    private void OnDeleteLayerCommandExecuted()
    {
        _model.DeleteLayer();
    }

    private bool CanDeleteLayerCommandExecute() => true;

    #endregion

    #region DuplicateLayer

    public ICommand DuplicateLayerCommand { get; set; }

    private void OnDuplicateLayerCommandExecuted()
    {
        _model.DuplicateLayer();
    }

    private bool CanDuplicateLayerCommandExecute() => true;

    #endregion

    #region SaveFileDialog

    public ICommand SaveFileDialogCommand { get; set; }

    private async Task OnSaveFileDialogCommandExecuted()
    {
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