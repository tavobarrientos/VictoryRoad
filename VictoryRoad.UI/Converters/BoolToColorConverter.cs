using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace VictoryRoad.UI.Converters;

public class BoolToColorConverter : IValueConverter
{
    public static readonly BoolToColorConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isValid)
        {
            return isValid ? Brushes.Green : Brushes.Red;
        }
        return Brushes.Black;
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}