using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace VictoryRoad.UI.Converters;

public class DateTimeToDateTimeOffsetConverter : IValueConverter
{
    public static readonly DateTimeToDateTimeOffsetConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return new DateTimeOffset(dateTime);
        }
        return null;
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset dateTimeOffset)
        {
            var dateTime = dateTimeOffset.DateTime;
            
            // Validate birth date is not too recent (must be at least 5 years ago)
            if (dateTime > DateTime.Now.AddYears(-5))
            {
                // Return the maximum allowed date instead of the invalid date
                return DateTime.Now.AddYears(-5);
            }
            
            // Validate birth date is not in the future
            if (dateTime > DateTime.Now)
            {
                return DateTime.Now;
            }
            
            return dateTime;
        }
        return null;
    }
}