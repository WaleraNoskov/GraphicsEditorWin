using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace GraphicsEditor.Infrastructure.Converters;

public class EmptyToEnabledConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is not ICollection collection)
            return Binding.DoNothing;

        return collection.Count != 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}