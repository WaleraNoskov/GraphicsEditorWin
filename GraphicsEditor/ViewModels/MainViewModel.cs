using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
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
    }

    public Mat OriginImage => _model.MainSpace.Original;
    
    public Mat EditedImage => _model.MainSpace.Filtered;
    
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
        }
    }

    public bool ImageIsOpened => _model.ImageIsOpened;
    
    #region ResetCommand

    public ICommand ResetCommand { get; set; }

    private void OnResetCommandExecuted()
    {
        _model.ResetAndReapplyFilters();
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
            Filter = "Image files (*.png;*.jpeg,*.jpg,*.bmp)|*.png;*.jpeg;*.jpg;*.bmp"
        };

        var result = dialog.ShowDialog();
        if (result is not true)
            return;

        _model.OpenImage(dialog.FileName);
    }

    private bool CanOpenImageDialogCommandExecute() => true;

    #endregion
}