using System.Globalization;
using System.Windows.Data;
using GraphicsEditor.Entities;

namespace GraphicsEditor.Infrastructure.Converters;

public class LayersConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is not ICollection<GraphicObject> collection)
            return null;

        var inverted = collection
            .Select((x, i) => new KeyValuePair<int, GraphicObject>())
            .OrderByDescending(x => x.Key)
            .ToList();

        return inverted;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}