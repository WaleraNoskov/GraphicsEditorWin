using System.Collections.ObjectModel;
using System.IO;
using GraphicsEditor.Entities;
using GraphicsEditor.Infrastructure;
using GraphicsEditor.Services;
using OpenCvSharp;

namespace GraphicsEditor.Models;

public class MainModel : PropertyObject
{
    private readonly IFiltersService _filtersService;
    private readonly ISavingService _savingService;
    
    private GraphicObject _graphicObject;

    public MainModel(IFiltersService filtersService, ISavingService savingService)
    {
        _filtersService = filtersService;
        _savingService = savingService;

        GraphicObject = new GraphicObject();
        ResetFilters();
    }
    
    public GraphicObject GraphicObject
    {
        get => _graphicObject;
        private set => SetField(ref _graphicObject, value);
    }

    public IReadOnlyDictionary<Filter, float> Filters { get; private set; }

    public void SetFilterAsync(Filter filter, float mix)
    {
        if (!GraphicObject.Filters.ContainsKey(filter))
            GraphicObject.Filters.TryAdd(filter, mix);

        GraphicObject.Filters[filter] = mix;

        ReApplyFilters();
        OnPropertyChanged(nameof(Filters));
    }

    public void OpenFile(string path)
    {
        GraphicObject.Dispose();
        
        GraphicObject = new GraphicObject($"Image", path);
        ResetFilters();
        ReApplyFilters();
        
        Filters = new ReadOnlyDictionary<Filter, float>(GraphicObject.Filters);
        OnPropertyChanged(nameof(Filters));
    }
    
    public async Task<bool> SaveImage(string path)
    {
        var extension = Path.GetExtension(path);

        return extension switch
        {
            ".jpeg" or ".jpg"
                => _savingService.SaveAsJpg(GraphicObject.Filtered, path, 100),
            ".png"
                => _savingService.SaveAsPng(GraphicObject.Filtered, path, 0),
            ".tiff"
                => _savingService.SaveAsTiff(GraphicObject.Filtered, path),
            _ => false
        };
    }
    
    public void ReApplyFilters()
    {
        GraphicObject.Filtered.Dispose();
        GraphicObject.Filtered = GraphicObject.Original.Clone();

        foreach (var filter in GraphicObject.Filters)
        {
            Action<Mat, float>? filteringAction = filter.Key switch
            {
                Filter.Grayscale => _filtersService.ApplyGrayscale,
                Filter.Alpha => _filtersService.ApplyAlpha,
                _ => null
            };

            filteringAction?.Invoke(GraphicObject.Filtered, filter.Value);
        }

        var foundContrast = GraphicObject.Filters.TryGetValue(Filter.Contrast, out var contrast);
        var foundBrightness = GraphicObject.Filters.TryGetValue(Filter.Brightness, out var brightness);

        _filtersService.ApplyBrightnessAndContrast(GraphicObject.Filtered, foundContrast ? contrast : 1, foundBrightness ? brightness : 0);
    }

    public void ResetFilters()
    {
        GraphicObject.Filters.Clear();

        GraphicObject.Filters.Add(Filter.Grayscale, DefaultFilterValues.DefaultGrayscale);
        GraphicObject.Filters.Add(Filter.Brightness, DefaultFilterValues.DefaultBrightness);
        GraphicObject.Filters.Add(Filter.Contrast, DefaultFilterValues.DefaultContrast);
        GraphicObject.Filters.Add(Filter.Alpha, DefaultFilterValues.DefaultAlpha);
    }
}