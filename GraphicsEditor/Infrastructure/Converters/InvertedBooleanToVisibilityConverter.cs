﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GraphicsEditor.Infrastructure.Converters;

public class InvertedBooleanToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
            return Visibility.Collapsed;
        
        return boolValue ? Visibility.Collapsed : Visibility.Visible;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}