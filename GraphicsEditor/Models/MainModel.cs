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
    private readonly ISelectionService _selectionService;

    public MainModel(IFiltersService filtersService, ISavingService savingService, ISelectionService selectionService)
    {
        _filtersService = filtersService;
        _savingService = savingService;
        _selectionService = selectionService;

        ProjectInfo = new ProjectInfo("Untitled");

        ResetAndReapplyFilters(ProjectInfo.Layers.First());
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
        var layer = new GraphicObject($"Layer {ProjectInfo.Layers.Count + 1}", path);

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
        if (SelectedLayer is null)
            return;

        var layerToDelete = SelectedLayer;
        var layerToDeleteIndex = ProjectInfo.Layers.IndexOf(layerToDelete);
        if (ProjectInfo.Layers.Count > 1 && layerToDeleteIndex > 0)
            SelectLayer(ProjectInfo.Layers[layerToDeleteIndex - 1]);
        if (ProjectInfo.Layers.Count > 1 && layerToDeleteIndex < ProjectInfo.Layers.Count - 1)
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

    public void CropLayer(Frame selectionArea)
    {
        Crop(SelectedLayer, selectionArea);
        OnPropertyChanged(nameof(ProjectInfo.Layers));
    }

    public void DivideNewLayer(Frame selectionArea)
    {
        var layer = (SelectedLayer.Clone() as GraphicObject)!;
        Crop(layer, selectionArea);

        _selectionService.CutSquare(SelectedLayer.Original, selectionArea);
        ReApplyAllFilters(SelectedLayer);

        ProjectInfo.Layers.Insert(ProjectInfo.Layers.IndexOf(SelectedLayer) + 1, layer);
        SelectLayer(layer);

        OnPropertyChanged(nameof(ProjectInfo.Layers));
    }

    private void Crop(GraphicObject layer, Frame selectionArea)
    {
        var normalized = new Frame
        {
            Y1 = Math.Min(selectionArea.Y1, selectionArea.Y2),
            X1 = Math.Min(selectionArea.X1, selectionArea.X2),
            Y2 = Math.Max(selectionArea.Y1, selectionArea.Y2),
            X2 = Math.Max(selectionArea.X1, selectionArea.X2),
        };

        var local = new Frame
        {
            X1 = normalized.X1 - layer.Left,
            Y1 = normalized.Y1 - layer.Top,
            X2 = normalized.X2 - layer.Left,
            Y2 = normalized.Y2 - layer.Top,
        };
        
        if(!(normalized.X1 < layer.Left + layer.Width && normalized.X2 > layer.Left &&
           normalized.Y1 < layer.Top + layer.Height && normalized.Y2 > layer.Top))
            return;
        
        var corrected = new Frame
        {
            X1 = Math.Max(0, local.X1),
            Y1 = Math.Max(0, local.Y1),
            X2 = Math.Min(layer.Width, local.X2),
            Y2 = Math.Min(layer.Height, local.Y2),
        };

        var cropped = _selectionService.GetSquare(layer.Original, corrected);

        layer.Top += corrected.Y1;
        layer.Left += corrected.X1;
        
        layer.Original.Dispose();
        layer.Original = cropped;

        layer.Filtered.Dispose();
        layer.Filtered = cropped.Clone();

        ReApplyAllFilters(layer);
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