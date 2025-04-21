using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using GraphicsEditor.Entities;
using GraphicsEditor.Infrastructure;
using GraphicsEditor.Models;
using OpenCvSharp;

namespace GraphicsEditor.ViewModels;

public class MainViewModel : PropertyObject
{
    private readonly MainModel _model;

    public MainViewModel(MainModel model)
    {
        _model = model;
        _model.PropertyChanged += (sender, args) => OnPropertyChanged(args.PropertyName ?? string.Empty);
    }

    public Mat OriginImage => _model.MainSpace.Original;
    
    public Mat EditedImage => _model.MainSpace.Filtered;
    
    public IReadOnlyDictionary<Filter, float> Filters => _model.Filters;

    public bool GrayscaleIsEnabled => Grayscale != DefaultFilterValues.DefaultGrayscale;
    public int Grayscale
    {
        get => _model.Filters[Filter.Grayscale].ToPercentage();
        set
        {
            _model.SetFilterAsync(Filter.Grayscale, value.FromPercentage()); 
            OnPropertyChanged(nameof(GrayscaleIsEnabled));
        }
    }

    public bool BrightnessIsEnabled => Brightness != DefaultFilterValues.DefaultBrightness;
    public int Brightness
    {
        get => (int)_model.Filters[Filter.Brightness];
        set
        {
            _model.SetFilterAsync(Filter.Brightness, value);
            OnPropertyChanged(nameof(BrightnessIsEnabled));
        }
    }

    public bool ContrastIsEnabled => Contrast != DefaultFilterValues.DefaultContrast;
    public int Contrast
    {
        get => (int)_model.Filters[Filter.Contrast].ToPercentage();
        set
        {
            _model.SetFilterAsync(Filter.Contrast, value.FromPercentage());
            OnPropertyChanged(nameof(ContrastIsEnabled));
        }
    }
}