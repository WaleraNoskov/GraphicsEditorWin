using System.Collections.ObjectModel;
using GraphicsEditor.Entities;
using GraphicsEditor.Infrastructure;
using GraphicsEditor.Services;
using OpenCvSharp;

namespace GraphicsEditor.Models;

public class MainModel : PropertyObject
{
    private readonly IFiltersService _filtersService;

    public MainModel(IFiltersService filtersService)
    {
        _filtersService = filtersService;
        MainSpace = new Space("C:/Users/coder5/Pictures/baboon.bmp");
        Filters = new ReadOnlyDictionary<Filter, float>(_mainSpace.Filters);
    }
    
    #region MainSpace

    private readonly Space _mainSpace;

    public Space MainSpace
    {
        get => _mainSpace;
        private init => SetField(ref _mainSpace, value);
    }

    #endregion
    
    public IReadOnlyDictionary<Filter, float> Filters { get; }

    public void SetFilterAsync(Filter filter, float mix)
    {
        if(!MainSpace.Filters.ContainsKey(filter))
            MainSpace.Filters.TryAdd(filter, mix);
        
        MainSpace.Filters[filter] = mix;

        ReApplyAllFiltersAsync();
        OnPropertyChanged(nameof(Filters));
    }

    private void RemoveFilterAsync(Filter filter)
    {
        MainSpace.Filters.Remove(filter);
        
        ReApplyAllFiltersAsync();
    }

    private void ReApplyAllFiltersAsync()
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
}