using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AutodartsTrainer.Services;

namespace AutodartsTrainer.Converters;

public class LocalizationKeyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string key && ApplicationLocalizationService is LocalizationService localization)
        {
            return localization.GetString(key);
        }

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

    private static LocalizationService? ApplicationLocalizationService =>
        Application.Current?.Resources["Localization"] as LocalizationService;
}
