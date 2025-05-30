﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GraphicsEditor.Infrastructure.Converters;

public class InvertedNullToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}