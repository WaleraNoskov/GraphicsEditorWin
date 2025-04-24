using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GraphicsEditor.Entities;
using GraphicsEditor.Infrastructure;
using GraphicsEditor.Services;
using Microsoft.Win32;
using OpenCvSharp;

namespace GraphicsEditor.Models;

public class MainModel : PropertyObject
{
    private readonly IFiltersService _filtersService;
    private readonly ISavingService _savingService;

    public MainModel(IFiltersService filtersService, ISavingService savingService)
    {
        _filtersService = filtersService;
        _savingService = savingService;

        ProjectInfo = new ProjectInfo("Untitled");

        SelectLayer(ProjectInfo.Layers.First());
    }

    #region ProjectInfo : ProjectInfo

    private ProjectInfo _projectInfo;
    
    public ProjectInfo ProjectInfo
    {
        get => _projectInfo;
        private set => SetField(ref _projectInfo, value);
    }

    #endregion ProjectInfo
    
    #region SelectedLayer

    private GraphicObject _selectedLayer;

    public GraphicObject SelectedLayer
    {
        get => _selectedLayer;
        private set => SetField(ref _selectedLayer, value);
    }

    #endregion

    public IReadOnlyDictionary<Filter, float> Filters { get; private set; }

    public void SetFilterAsync(Filter filter, float mix)
    {
        if (!SelectedLayer.Filters.ContainsKey(filter))
            SelectedLayer.Filters.TryAdd(filter, mix);

        SelectedLayer.Filters[filter] = mix;

        ReApplyAllFilters(SelectedLayer);
        OnPropertyChanged(nameof(Filters));
    }

    public void ResetAndReapplyFilters(GraphicObject? layer = null)
    {
        ResetFilters(layer ?? SelectedLayer);
        ReApplyAllFilters(layer ?? SelectedLayer);

        OnPropertyChanged(nameof(Filters));
    }

    public void AddLayerFromFile(string path)
    {
        var layer = new GraphicObject($"Layer {ProjectInfo.Layers.Count + 1}",path);

        if (!ProjectInfo.Layers.Any())
        {
            ProjectInfo.Width = layer.Original.Width;
            ProjectInfo.Height = layer.Original.Height;
        }

        ProjectInfo.Layers.Add(layer);
        ResetAndReapplyFilters(layer);
        SelectLayer(layer);
        
        OnPropertyChanged(nameof(ProjectInfo.Width));
        OnPropertyChanged(nameof(ProjectInfo.Height));
        OnPropertyChanged(nameof(ProjectInfo.Layers));
    }

    public void DeleteLayer()
    {
        if(SelectedLayer is null)
            return;
        
        var layerToDelete = SelectedLayer;
        var layerToDeleteIndex = ProjectInfo.Layers.IndexOf(layerToDelete);
        if(ProjectInfo.Layers.Count > 1 && layerToDeleteIndex > 0)
            SelectLayer(ProjectInfo.Layers[layerToDeleteIndex - 1]);
        if(ProjectInfo.Layers.Count > 1 && layerToDeleteIndex < ProjectInfo.Layers.Count - 1)
            SelectLayer(ProjectInfo.Layers[layerToDeleteIndex + 1]);
        
        ProjectInfo.Layers.Remove(layerToDelete);
        layerToDelete.Dispose();
        
        OnPropertyChanged(nameof(ProjectInfo.Layers));
    }

    public void DuplicateLayer()
    {
        if (SelectedLayer is null)
            return;

        var duplicate = (GraphicObject)SelectedLayer.Clone();
        ProjectInfo.Layers.Add(duplicate);
        SelectLayer(duplicate);
        ReApplyAllFilters(duplicate);
        
        OnPropertyChanged(nameof(ProjectInfo.Layers));
    }
    
    public async Task<bool> SaveImage(string path)
    {
        var extension = Path.GetExtension(path);

        return extension switch
        {
            ".jpeg" or ".jpg"
                => _savingService.SaveAsJpg(SelectedLayer.Filtered, path, 100),
            ".png"
                => _savingService.SaveAsPng(SelectedLayer.Filtered, path, 0),
            ".tiff"
                => _savingService.SaveAsTiff(SelectedLayer.Filtered, path),
            _ => false
        };
    }

    public void SelectLayer(GraphicObject layer)
    {
        SelectedLayer = layer;
        Filters = new ReadOnlyDictionary<Filter, float>(SelectedLayer?.Filters ?? new Dictionary<Filter, float>());
        
        OnPropertyChanged(nameof(Filters));
    }
    
    private void ReApplyAllFilters(GraphicObject layer)
    {
        layer.Filtered.Dispose();
        layer.Filtered = layer.Original.Clone();

        foreach (var filter in layer.Filters)
        {
            Action<Mat, float>? filteringAction = filter.Key switch
            {
                Filter.Grayscale => _filtersService.ApplyGrayscale,
                Filter.Alpha => _filtersService.ApplyAlpha,
                _ => null
            };

            filteringAction?.Invoke(layer.Filtered, filter.Value);
        }

        var foundContrast = layer.Filters.TryGetValue(Filter.Contrast, out var contrast);
        var foundBrightness = layer.Filters.TryGetValue(Filter.Brightness, out var brightness);

        _filtersService.ApplyBrightnessAndContrast(layer.Filtered, foundContrast ? contrast : 1, foundBrightness ? brightness : 0);
    }

    private static void ResetFilters(GraphicObject layer)
    {
        layer.Filters.Clear();

        layer.Filters.Add(Filter.Grayscale, DefaultFilterValues.DefaultGrayscale);
        layer.Filters.Add(Filter.Brightness, DefaultFilterValues.DefaultBrightness);
        layer.Filters.Add(Filter.Contrast, DefaultFilterValues.DefaultContrast);
        layer.Filters.Add(Filter.Alpha, DefaultFilterValues.DefaultAlpha);
    }
}