using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using GraphicsEditor.Core.Entities;
using GraphicsEditor.Core.Models;
using OpenCvSharp;

namespace GraphicsEditor.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly MainModel _model;

    public MainViewModel(MainModel model)
    {
        _model = model;
        _model.PropertyChanged += (sender, args) => OnPropertyChanged(args.PropertyName ?? string.Empty);
        
        ApplyGrayscaleCommand = new RelayCommand(OnApplyGrayscaleCommandExecuted, CanApplyGrayscaleCommandExecute);
    }

    public Mat OriginImage => _model.MainSpace.Original;
    
    public Mat EditedImage => _model.MainSpace.Filtered;
    
    public IDictionary<Filter, float> Filters => _model.Filters;
    
    #region ApplyGrayscale

    public ICommand ApplyGrayscaleCommand { get; set; }

    private void OnApplyGrayscaleCommandExecuted()
    {
        _model.SetFilterAsync(Filter.Grayscale, 1);
    }

    private bool CanApplyGrayscaleCommandExecute() => true;

    #endregion
}