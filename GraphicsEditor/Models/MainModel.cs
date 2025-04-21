using System.Collections.ObjectModel;
using GraphicsEditor.Entities;
using GraphicsEditor.Infrastructure;
using GraphicsEditor.Services;
using Microsoft.Win32;
using OpenCvSharp;

namespace GraphicsEditor.Models;

public class MainModel : PropertyObject
{
    private readonly IFiltersService _filtersService;

    public MainModel(IFiltersService filtersService)
    {
        _filtersService = filtersService;

        MainSpace = new Space();
        ResetFilters();
        Filters = new ReadOnlyDictionary<Filter, float>(MainSpace.Filters);
    }
    
    #region MainSpace

    private Space _mainSpace;

    public Space MainSpace
    {
        get => _mainSpace;
        private set => SetField(ref _mainSpace, value);
    }

    #endregion

    #region ImageIsOpened : bool

    private bool _imageIsOpened;

    /// <summary> 
    /// description 
    /// </summary>
    public bool ImageIsOpened
    {
        get => _imageIsOpened;
        set => SetField(ref _imageIsOpened, value);
    }

    #endregion ImageIsOpened
    
    public IReadOnlyDictionary<Filter, float> Filters { get; private set; }

    public void SetFilterAsync(Filter filter, float mix)
    {
        if(!MainSpace.Filters.ContainsKey(filter))
            MainSpace.Filters.TryAdd(filter, mix);
        
        MainSpace.Filters[filter] = mix;

        ReApplyAllFilters();
        OnPropertyChanged(nameof(Filters));
    }

    public void ResetAndReapplyFilters()
    {
        ResetFilters();
        ReApplyAllFilters();
        
        OnPropertyChanged(nameof(Filters));
    }

    public void OpenImage(string path)
    {
        MainSpace?.Dispose();
        MainSpace = new Space(path);
        ImageIsOpened = true;
        
        Filters = new ReadOnlyDictionary<Filter, float>(_mainSpace.Filters);
        ResetAndReapplyFilters();
    }
    
    private void ReApplyAllFilters()
    {
        MainSpace.Filtered.Dispose();
        MainSpace.Filtered = MainSpace.Original.Clone();

        foreach (var filter in MainSpace.Filters)
        {
            Action<Mat, float>? filteringAction = filter.Key switch
            {
                Filter.Grayscale => _filtersService.ApplyGrayscale,
                Filter.Alpha => _filtersService.ApplyAlpha,
                _ => null
            };
        
            filteringAction?.Invoke(MainSpace.Filtered, filter.Value);
        }

        var foundContrast = MainSpace.Filters.TryGetValue(Filter.Contrast, out var contrast);
        var foundBrightness = MainSpace.Filters.TryGetValue(Filter.Brightness, out var brightness);
        
        _filtersService.ApplyBrightnessAndContrast(MainSpace.Filtered, foundContrast ? contrast : 1, foundBrightness ? brightness : 0);
    }

    private void ResetFilters()
    {
        MainSpace.Filters.Clear();
        
        MainSpace.Filters.Add(Filter.Grayscale, DefaultFilterValues.DefaultGrayscale);
        MainSpace.Filters.Add(Filter.Brightness, DefaultFilterValues.DefaultBrightness);
        MainSpace.Filters.Add(Filter.Contrast, DefaultFilterValues.DefaultContrast);
        MainSpace.Filters.Add(Filter.Alpha, DefaultFilterValues.DefaultAlpha);
    }
}